using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Sortingtime.Models;
using System.Diagnostics;
using Sortingtime.PdfMailWebJob.Infrastructure;
using System.Threading;
using Sortingtime.QueueModels;
using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using System.Net.Mail;
using Sortingtime.ApiModels;
using Sortingtime.PdfMailWebJob.Infrastructure.Extension;
using Sortingtime.PdfMailWebJob.HtmlGenerators;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sortingtime.PdfMailWebJob.Functions
{
    public class ReportFunction
    {
        private readonly ILogger logger;
        private readonly ApplicationDbContext dbContext;
        private readonly EmailMessageProvider emailMessageProvider;

        public ReportFunction(ILogger<ReportFunction> logger, ApplicationDbContext dbContext, EmailMessageProvider emailMessageProvider)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.emailMessageProvider = emailMessageProvider;
        }

        public async Task SendReportPdfAsync(
            [QueueTrigger("sendreportpdf")] ReportQueue reportMessage,
            [Blob("report/{ReportId}.pdf", FileAccess.Write)] CloudBlockBlob reportPdfOutput,
#if DEBUG
            [Blob("report/{ReportId}.html", FileAccess.Write)] CloudBlockBlob reportHtmlOutput,
#endif
            CancellationToken cancellationToken)
        {
            try
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                logger.LogTrace($"SendReport trigger event recived, ID: {reportMessage?.ReportId}.");

                if (string.IsNullOrEmpty(reportMessage?.CultureName)) throw new ArgumentNullException(nameof(ReportQueue.CultureName));

                CultureHandler.SetCulture(reportMessage.CultureName);
                var translate = new Translate();

                var report = dbContext.Reports.Where(r => (r.Status == ReportStatus.Created || r.Status == ReportStatus.Resending) &&
                    r.PartitionId == reportMessage.PartitionId && r.Id == reportMessage.ReportId).FirstOrDefault();
                if (report == null)
                {
                    throw new Exception("Report do not exists or has invalid status and is therefore not send.");
                }
                var reportData = await report.ReportData.FromJson<ReportDataApi>();
                var organization = dbContext.Organizations.Where(o => o.Id == reportMessage.PartitionId).Select(o => new { o.Name, o.Address, o.Logo }).FirstOrDefault();

                using (var pdfReportHtmlStream = await ReportHtml.CreateReportStream(translate, reportData.ShowGroupColl, organization?.Logo, organization?.Name, organization?.Address, reportData.ReportTitle, reportData.ReportSubTitle, reportData.ReportText, reportData.Report))
                {
#if DEBUG
                    await reportHtmlOutput.UploadFromStreamAsync(pdfReportHtmlStream);
                    pdfReportHtmlStream.Position = 0;
#endif

                    using (var reportPdfStream = new MemoryStream())
                    {
                        logger.LogTrace($"Before create PDF: {reportMessage?.ReportId}.");
                        PdfProvider.CreatePdfAsync(reportPdfStream, pdfReportHtmlStream, cancellationToken: cancellationToken);
                        logger.LogTrace($"After create PDF: {reportMessage?.ReportId}.");

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await SendEmailReport(reportPdfStream, translate, report);
                            await UpdateReportStatus(reportMessage.PartitionId, reportMessage.ReportId, report.Status == ReportStatus.Created ? ReportStatus.Send : ReportStatus.Resend);
                        }

                       await reportPdfOutput.UploadFromStreamAsync(reportPdfStream);
                    }
                }

                logger.LogTrace($"SendReport report send, ID: {reportMessage?.ReportId}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"SendReport failed, ID: {reportMessage?.ReportId}.");
                throw;
            }
        }

        private async Task SendEmailReport(MemoryStream reportPdfStream, Translate translate, Report report)
        {
            var subject = Encoding.UTF8.GetBytes($"{report.EmailSubject} #{report.Number}");
            var emailHtml = Encoding.UTF8.GetBytes(report.EmailBody.ToEmailHtml());
            
            await emailMessageProvider.SendEmailAsync(
                report.ToEmail.ToMailAddressArray(),
                Encoding.UTF8.GetString(subject),
                Encoding.UTF8.GetString(emailHtml),
                fromEmailAsCc: new MailAddress(report.FromEmail),
                attachmentName: string.Format("{0} {1}.pdf", translate.Get("REPORT.FILENAME"), report.Number),
                attachmentStream: reportPdfStream);

            reportPdfStream.Position = 0;
        }       

        private async Task UpdateReportStatus(long currentPartitionId, long reportId, ReportStatus status)
        {
            var report = dbContext.Reports.Where(r => r.PartitionId == currentPartitionId && r.Id == reportId).Single();
            report.Status = status;
            report.UpdateTimestamp = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}
