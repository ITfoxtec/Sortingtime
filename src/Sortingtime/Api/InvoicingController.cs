using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Sortingtime.Api.Logic;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
using Sortingtime.QueueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class InvoicingController : SecureApiController
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger logger;

        public InvoicingController(TelemetryClient telemetryClient, ILogger<InvoicingController> logger, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.telemetryClient = telemetryClient;
            this.logger = logger;
        }

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

            var invoice = new InvoiceApi
            {
                FromDate = fromDateValue,
                ToDate = toDateValue,
            };

            var partitionId = CurrentPartitionId;

            invoice.GroupTasks = DbContext.TaskItems.Where(ti => ti.Task.Group.PartitionId == partitionId && ti.Time > 0 && ti.Date >= fromDateValue && ti.Date <= toDateValue)
                .OrderBy(ti => ti.Task.Group.Name).ThenBy(ti => ti.Task.Name).GroupBy(ti => new { Group = ti.Task.Group.Name, Task = ti.Task.Name })
                    .Select(gw => new InvoiceGroupTaskApi
                    {
                        Group = gw.Key.Group,
                        Task = gw.Key.Task,
                        Time = gw.Sum(ti => ti.Time),
                        Users = gw.OrderBy(ti => ti.User.FullName).GroupBy(ti => ti.UserId)
                            .Select(ggw => new InvoiceUserApi
                            {
                                Id = ggw.Key,
                                FullName = ggw.Select(ti => ti.User.FullName).FirstOrDefault(),
                                Time = ggw.Sum(ti => ti.Time),
                            }).ToList()
                    }).AsNoTracking().ToList();

            var hourPriceLogic = new HourPriceLogic(DbContext, CurrentPartitionId);

            invoice.TotalTime = invoice.GroupTasks.Sum(gt => gt.Time);

            invoice.SubTotalPrice = 0;
            foreach (var gt in invoice.GroupTasks)
            {
                foreach (var user in gt.Users)
                {
                    user.HourPrice = hourPriceLogic.GetHourPrice(gt.Group, gt.Task, user.Id).Result?.HourPrice;
                    if (user.HourPrice.HasValue)
                    {
                        user.Price = user.HourPrice.Value * user.Time / 60;
                        invoice.SubTotalPrice += user.Price.Value;
                    }
                }
                gt.Price = gt.Users.Sum(u => u.Price);
            }

            return Json(invoice);
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]InvoiceAndContentApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var translate = new Translate();

            await CleanWhiteSpace(item);

            if (!item.ToEmail.EmailsIsValid())
            {
                ModelState.AddModelError("item.ToEmail", translate.Get("INVOICE.EMAIL_COMMA_ERROR"));
                return new BadRequestObjectResult(ModelState);
            }

            await LoadUser(item);

            Invoice invoice = null;

            if (item.RelatedId.HasValue)
            {
                invoice = await ResendInvoice(item, item.RelatedId.Value);
                if (invoice == null)
                {
                    return new NotFoundResult();
                }
            }
            else
            {
                var maxInvoices = await DbContext.Partitions.Where(p => p.Id == CurrentPartitionId).Select(p => p.MaxInvoicesPerMonth).FirstAsync();
                // Max Invoices count
                if (await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.InvoiceDate >= item.InvoiceDate.FirstDayOfMonth() && i.InvoiceDate < item.InvoiceDate.FirstDayOfNextMonth()).CountAsync() >= maxInvoices)
                {
                    ModelState.AddModelError("", translate.Get("INVOICE.MAXIMUM_INVOICES_ERROR"));
                    return new BadRequestObjectResult(ModelState);
                }

                invoice = await CreateAndSendInvoice(item);
            }

            QueueInvoice(invoice);
            telemetryClient.TrackEvent("InvoiceQueued", new Dictionary<string, string> { { "UserId", Convert.ToString(CurrentUserId) }, { "PartitionId", CurrentPartitionIdAsString } });

            return Json(new InvoiceItemApi
            {
                Id = invoice.Id,
                Status = invoice.Status,
                Number = invoice.Number,
                CustomerShort = invoice.CustomerShort,
                InvoiceDate = invoice.InvoiceDate,
                SubTotalPrice = invoice.SubTotalPrice,
                ToEmail = invoice.ToEmail,
            });
        }

        private async Task<Invoice> CreateAndSendInvoice(InvoiceAndContentApi item)
        {
            var invoice = Invoice.CreateNew(CurrentPartitionId, CurrentUserId, await NextInvoiceNumber());
            invoice.CustomerShort = item.InvoiceCustomer?.CleanWhiteSpace()?.FirstLine();
            invoice.SubTotalPrice = item.Invoice.SubTotalPrice;
            await MapDeltaValuesAndCleanWhiteSpace(item, invoice);

            invoice.InvoiceData = await new InvoiceDataApi
            {
                ShowGroupColl = item.showGroupColl,
                InvoiceTitle = item.InvoiceTitle?.CleanWhiteSpace(),
                InvoiceCustomer = item.InvoiceCustomer?.CleanWhiteSpace(),
                VatNumber = item.Vat ? item.VatNumber?.CleanWhiteSpace() : null,
                PaymentDetails = item.PaymentDetails?.CleanWhiteSpace(),
                InvoicePaymentTerms = item.InvoicePaymentTerms?.CleanWhiteSpace(),
                InvoiceReference = item.InvoiceReference?.CleanWhiteSpace(),
                InvoiceText = item.InvoiceText?.CleanWhiteSpace(),
                Tax = item.Tax,
                Vat = item.Vat,
                TaxPercentage = item.Tax ? item.TaxPercentage : null,
                VatPercentage = item.Vat ? item.VatPercentage : null,
                TaxPrice = item.Tax ? item.TaxPrice : null,
                VatPrice = item.Vat ? item.VatPrice : null,
                TotalPrice = item.TotalPrice,
                Invoice = item.Invoice,
            }.ToJson();

            DbContext.Invoices.Add(invoice);
            await DbContext.SaveChangesAsync();

            return invoice;
        }

        private async Task<Invoice> ResendInvoice(InvoiceAndContentApi item, long invoiceId)
        {
            var invoice = await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.Id == invoiceId).FirstOrDefaultAsync();
            if(invoice != null)
            {
                invoice.Status = InvoiceStatus.Resending;
                invoice.UpdateTimestamp = DateTime.UtcNow;
                invoice.ToEmail = item.ToEmail;
                invoice.FromFullName = item.FromFullName;
                invoice.FromEmail = item.FromEmail;
                invoice.EmailSubject = item.EmailSubject;
                invoice.EmailBody = item.EmailBody;
                await DbContext.SaveChangesAsync();
            }
            return invoice;
        }

        private void QueueInvoice(Invoice invoice)
        {
            var storageAccount = CloudStorageAccount.Parse(Startup.Configuration.GetConnectionString("AzureWebJobsStorage"));
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            var requestQueue = cloudQueueClient.GetQueueReference("sendinvoicepdf");
            requestQueue.CreateIfNotExists();
            var requestMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new InvoiceQueue
            {
                PartitionId = CurrentPartitionId,
                InvoiceId = invoice.Id,
                CultureName = CultureInfo.CurrentCulture.Name
            }));
            requestQueue.AddMessage(requestMessage);
        }

        private async Task<long> NextInvoiceNumber()
        {
            var invoiceNumber = await DbContext.Invoices.Where(r => r.PartitionId == CurrentPartitionId).OrderByDescending(r => r.Number).Select(r => r.Number).FirstOrDefaultAsync();

            if (invoiceNumber <= 0)
            {
                invoiceNumber = 1;
            }
            else
            {
                 invoiceNumber++;
            }

            var firstInvoiceNumber = await DbContext.Organizations.Where(o => o.Id == CurrentPartitionId).Select(o => o.FirstInvoiceNumber).FirstOrDefaultAsync();
            if (firstInvoiceNumber.HasValue && invoiceNumber < firstInvoiceNumber.Value)
            {
                invoiceNumber = firstInvoiceNumber.Value;
            }

            return invoiceNumber;
        }

        private async Task LoadUser(InvoiceAndContentApi item)
        {
            var user = await DbContext.Users.Where(u => u.Id == CurrentUserId).Select(u => new { u.FullName, u.Email }).FirstAsync();

            item.FromFullName = user.FullName;
            item.FromEmail = user.Email;
        }       
    }
}
