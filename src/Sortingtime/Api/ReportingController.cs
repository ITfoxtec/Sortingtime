using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Sortingtime.QueueModels;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]    
    public class ReportingController : SecureApiController
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger logger;

        public ReportingController(TelemetryClient telemetryClient, ILogger<ReportingController> logger, ApplicationDbContext dbContext) : base(dbContext)
		{
            this.telemetryClient = telemetryClient;
            this.logger = logger;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                if (!fromDate.HasValue)
                    ModelState.AddModelError("fromDate", "Required Date is null or empty.");
                if (!toDate.HasValue)
                    ModelState.AddModelError("toDate", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }

            var dateSpanDays = (toDate.Value - fromDate.Value).TotalDays + 1;
            if (dateSpanDays > 31)
            {
                ModelState.AddModelError("", "Date timespan is more than 31 days.");
                return new BadRequestObjectResult(ModelState);
            }

            var fromDateValue = fromDate.Value;
            var toDateValue = toDate.Value;
            var partitionId = CurrentPartitionId;

            var report = new ReportApi
            {
                FromDate = fromDateValue,
                ToDate = toDateValue,
                DaysInMonth = (int)dateSpanDays,
            };

            report.GroupTaskTotals = await EntityFrameworkQueryableExtensions.ToListAsync(DbContext.TaskItems.Where(ti => ti.Task.Group.PartitionId == partitionId && ti.Time > 0 && ti.Date >= fromDateValue && ti.Date <= toDateValue)
                .OrderBy(ti => ti.Task.Group.Name).ThenBy(ti => ti.Task.Name).GroupBy(ti => new { Group = ti.Task.Group.Name, Task = ti.Task.Name })
                    .Select(gw => new ReportGroupTaskApi
                    {
                        Group = gw.Key.Group,
                        Task = gw.Key.Task,
                        MonthTotal = gw.Sum(ti => ti.Time),
                    }).AsNoTracking());

            // Ny version der udnytter grupper opdelingen...
            //var groups = DbContext.Groups.Where(g => g.PartitionId == partitionId && g.Tasks.Any(t => t.Items.Where(i => i.Date >= fromDateValue && i.Date <= toDateValue).Count() > 0));
            //report.GroupTaskTotals = groups.Select(g => new ReportGroupTaskApi
            //{
            //    Group = g.Name,
            //    Task = g.Tasks.Select(t => t.Name).FirstOrDefault(),
            //});

            report.MonthTotal = await EntityFrameworkQueryableExtensions.SumAsync(DbContext.TaskItems.Where(ti => ti.Task.Group.PartitionId == partitionId && ti.Time > 0 && ti.Date >= fromDateValue && ti.Date <= toDateValue)
                .Select(ti => (int)ti.Time));

            report.Users = DbContext.TaskItems.Where(ti => ti.Task.Group.PartitionId == partitionId && ti.Time > 0 && ti.Date >= fromDateValue && ti.Date <= toDateValue)
                .OrderBy(ti => ti.User.FullName).GroupBy(ti => new { ti.User })
                     .Select(gw => new ReportUserApi
                     {
                         Id = gw.Key.User.Id,
                         FullName = gw.Key.User.FullName,
                         GroupTasks = gw.OrderBy(ti => ti.Task.Group.Name).ThenBy(ti => ti.Task.Name).GroupBy(ti => new { Group = ti.Task.Group.Name, Task = ti.Task.Name })
                            .Select(ggw => new ReportGroupTaskApi
                            {
                                Group = ggw.Key.Group,
                                Task = ggw.Key.Task,
                                Works = ggw.GroupBy(ti => ti.Date)
                                    .Select(gdw => new ReportWorkApi
                                    {
                                        Day = gdw.Key.Day,
                                        Time = gdw.Sum(ti => ti.Time)
                                    }).OrderBy(gdw => gdw.Day).ToList(),
                                MonthTotal = ggw.Sum(ti => (int)ti.Time)
                            }).ToList(),
                         MonthTotal = gw.Sum(ti => ti.Time)
                     }).OrderBy(iu => iu.FullName).AsNoTracking().ToList();

            return Json(report);
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]ReportAndContentApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var translate = new Translate();

            await CleanWhiteSpace(item);

            if (!item.ToEmail.EmailsIsValid())
            {
                ModelState.AddModelError("item.ToEmail", translate.Get("REPORT.EMAIL_COMMA_ERROR"));
                return new BadRequestObjectResult(ModelState);
            }

            await LoadUser(item);

            Report report = null;
            if (item.RelatedId.HasValue)
            {
                report = await ResendReport(item, item.RelatedId.Value);
                if (report == null)
                {
                    return new NotFoundResult();
                }
            }
            else
            {
                var maxReports = await DbContext.Partitions.Where(p => p.Id == CurrentPartitionId).Select(p => p.MaxReportsPerMonth).FirstAsync();
                // Max Reports count
                if (await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.ToDate >= item.Report.ToDate.FirstDayOfMonth() && r.ToDate < item.Report.ToDate.FirstDayOfNextMonth()).CountAsync() >= maxReports)
                {
                    ModelState.AddModelError("", translate.Get("REPORT.MAXIMUM_REPORTS_ERROR"));
                    return new BadRequestObjectResult(ModelState);
                }

                report = await CreateAndSendReport(item);
            }

            QueueReport(report);
            telemetryClient.TrackEvent("ReportQueued", new Dictionary<string, string> { { "UserId", Convert.ToString(CurrentUserId) }, { "PartitionId", CurrentPartitionIdAsString } });

            return Json(new ReportItemApi
            {
                Id = report.Id,
                Status = report.Status,
                Number = report.Number,
                TotalTime = report.TotalTime,
                ToEmail = report.ToEmail,
            });
        }

        private async Task<Report> CreateAndSendReport(ReportAndContentApi item)
        {
            var report = Report.CreateNew(CurrentPartitionId, CurrentUserId, await NextReportNumber());
            report.FromDate = item.Report.FromDate;
            report.ToDate = item.Report.ToDate;
            report.TotalTime = item.Report.MonthTotal;
            await MapDeltaValuesAndCleanWhiteSpace(item, report);

            report.ReportData = await new ReportDataApi
            {
                ShowGroupColl = item.showGroupColl,
                ReportTitle = item.ReportTitle,
                ReportSubTitle = item.ReportSubTitle,
                ReportText = item.ReportText,
                Report = item.Report,
            }.ToJson();

            DbContext.Reports.Add(report);
            await DbContext.SaveChangesAsync();

            return report;
        }

        private async Task<Report> ResendReport(ReportAndContentApi item, long reportId)
        {
            var report = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.Id == reportId).FirstOrDefaultAsync();
            if (report != null)
            {
                report.Status = ReportStatus.Resending;
                report.UpdateTimestamp = DateTime.UtcNow;
                report.ToEmail = item.ToEmail;
                report.FromFullName = item.FromFullName;
                report.FromEmail = item.FromEmail;
                report.EmailSubject = item.EmailSubject;
                report.EmailBody = item.EmailBody;
                await DbContext.SaveChangesAsync();
            }
            return report;
        }

        private void QueueReport(Report report)
        {
            var storageAccount = CloudStorageAccount.Parse(Startup.Configuration.GetConnectionString("AzureWebJobsStorage"));
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            var requestQueue = cloudQueueClient.GetQueueReference("sendreportpdf");
            requestQueue.CreateIfNotExists();
            var requestMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new ReportQueue
            {
                PartitionId = CurrentPartitionId,
                ReportId = report.Id,
                CultureName = CultureInfo.CurrentCulture.Name
            }));
            requestQueue.AddMessage(requestMessage);
        }

        private async Task<long> NextReportNumber()
        {
            var reportNumber = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId).OrderByDescending(r => r.Number).Select(r => r.Number).FirstOrDefaultAsync();

            if(reportNumber <= 0)
            {
                return 1;
            }
            else
            {
                return reportNumber + 1;
            }
        }

        private async Task LoadUser(ReportAndContentApi item)
        {
            var user = await DbContext.Users.Where(u => u.Id == CurrentUserId).Select(u => new { u.FullName, u.Email } ).FirstAsync();

            item.FromFullName = user.FullName;
            item.FromEmail = user.Email;
        }
    }
}
