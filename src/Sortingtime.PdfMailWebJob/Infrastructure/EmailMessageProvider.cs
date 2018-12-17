using Sortingtime.Infrastructure;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.PdfMailWebJob.Infrastructure
{
    public class EmailMessageProvider
    {
        private readonly TextWriter log;
        private readonly EmailMessageClient emailMessageClient;

        public EmailMessageProvider(TextWriter log)
        {
            this.log = log;
            emailMessageClient = new EmailMessageClient
            {
                DefaultFromEmail = ConfigurationManager.AppSettings["Mail:DefaultFromEmail"],
                ApiKey = ConfigurationManager.AppSettings["Mail:ApiKey"],
                MessageSendtAudit = auditValue => log.WriteLine(auditValue),
            };
        }

        public async Task SendEmailAsync(MailAddress[] toEmail, string subject, string htmlBody, MailAddress fromEmail = null, string attachmentName = null, MemoryStream attachmentStream = null)
        {
            await emailMessageClient.SendEmailAsync(toEmail, subject, htmlBody, fromEmail, attachmentName, attachmentStream);            
        }
    }
}
