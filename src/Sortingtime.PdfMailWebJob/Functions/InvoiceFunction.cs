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
    public class InvoiceFunction
    {
        private readonly ILogger logger;
        private readonly ApplicationDbContext dbContext;
        private readonly EmailMessageProvider emailMessageProvider;

        public InvoiceFunction(ILogger<InvoiceFunction> logger, ApplicationDbContext dbContext, EmailMessageProvider emailMessageProvider)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.emailMessageProvider = emailMessageProvider;
        }

        public async Task SendInvoicePdfAsync(
            [QueueTrigger("sendinvoicepdf")] InvoiceQueue invoiceMessage,
            [Blob("invoice/{InvoiceId}.pdf", FileAccess.Write)] CloudBlockBlob invoicePdfOutput,
//#if DEBUG
//            [Blob("invoice/{InvoiceId}.html", FileAccess.Write)] Stream invoiceHtmlOutput,
//#endif
            CancellationToken cancellationToken)
        {
            try
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                logger.LogTrace($"SendInvoice trigger event recived, ID: {invoiceMessage?.InvoiceId}.");

                if (string.IsNullOrEmpty(invoiceMessage?.CultureName)) throw new ArgumentNullException(nameof(InvoiceQueue.CultureName));

                CultureHandler.SetCulture(invoiceMessage.CultureName);
                var translate = new Translate();

                var invoice = dbContext.Invoices.Where(i => (i.Status == InvoiceStatus.Created || i.Status == InvoiceStatus.Resending) &&
                    i.PartitionId == invoiceMessage.PartitionId && i.Id == invoiceMessage.InvoiceId).FirstOrDefault();
                if (invoice == null)
                {
                    throw new Exception("Invoice do not exists or have invalid status and is therefore not send.");
                }
                var invoiceData = await invoice.InvoiceData.FromJson<InvoiceDataApi>();
                var organization = dbContext.Organizations.Where(o => o.Id == invoiceMessage.PartitionId).Select(o => new { o.Name, o.Address, o.Logo, o.VatNumber }).FirstOrDefault();

                using (var pdfInvoiceHtmlStream = await InvoiceHtml.CreateInvoiceStream(translate, invoice, invoiceData, organization?.Logo, organization?.Name, organization?.Address))
                {
//#if DEBUG
//                    pdfInvoiceHtmlStream.CopyTo(invoiceHtmlOutput);
//                    pdfInvoiceHtmlStream.Position = 0;
//#endif

                    using (var invoicePdfStream = new MemoryStream())
                    {
                        logger.LogTrace($"Before create PDF: {invoiceMessage?.InvoiceId}.");
                        PdfProvider.CreatePdfAsync(invoicePdfStream, pdfInvoiceHtmlStream, cancellationToken: cancellationToken);
                        logger.LogTrace($"After create PDF: {invoiceMessage?.InvoiceId}.");

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await SendEmailInvoice(invoicePdfStream, translate, invoice);
                            await UpdateInvoiceStatus(invoiceMessage.PartitionId, invoiceMessage.InvoiceId, invoice.Status == InvoiceStatus.Created ? InvoiceStatus.Send : InvoiceStatus.Resend);
                        }

                        await invoicePdfOutput.UploadFromStreamAsync(invoicePdfStream);
                    }
                }

                logger.LogTrace($"SendInvoice invoice send, ID: {invoiceMessage?.InvoiceId}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"SendInvoice failed, ID: {invoiceMessage?.InvoiceId}.");
                throw;
            }
        }

        private async Task SendEmailInvoice(MemoryStream invoicePdfStream, Translate translate, Invoice invoice)
        {
            var subject = Encoding.UTF8.GetBytes($"{invoice.EmailSubject} #{invoice.Number}");
            var emailHtml = Encoding.UTF8.GetBytes(invoice.EmailBody.ToEmailHtml());

            await emailMessageProvider.SendEmailAsync(
                invoice.ToEmail.ToMailAddressArray(),
                Encoding.UTF8.GetString(subject),
                Encoding.UTF8.GetString(emailHtml),
                fromEmail: new MailAddress(invoice.FromEmail, invoice.FromFullName),
                attachmentName: string.Format("{0} {1}.pdf", translate.Get("INVOICE.FILENAME"), invoice.Number),
                attachmentStream: invoicePdfStream);

            invoicePdfStream.Position = 0;
        }
       
        private async Task UpdateInvoiceStatus(long currentPartitionId, long invoiceId, InvoiceStatus status)
        {
            var invoice = dbContext.Invoices.Where(i => i.PartitionId == currentPartitionId && i.Id == invoiceId).Single();
            invoice.Status = status;
            invoice.UpdateTimestamp = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}
