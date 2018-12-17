using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public class EmailMessageProvider
    {
        private readonly ILogger logger;
        private readonly EmailMessageClient emailMessageClient;

        public EmailMessageProvider(ILogger<EmailMessageProvider> logger)
        {
            this.logger = logger;
            emailMessageClient = new EmailMessageClient
            {
                DefaultFromEmail = Startup.Configuration["Mail:DefaultFromEmail"],
                ApiKey = Startup.Configuration["Mail:ApiKey"],
                MessageSendtAudit = auditValue => logger.LogInformation(auditValue),
            };
        }

        public async Task SendEmailAsync(MailAddress[] toEmail, string subject, string htmlBody, MailAddress fromEmail = null, string attachmentName = null, MemoryStream attachmentStream = null)
        {
            try
            {
                await emailMessageClient.SendEmailAsync(toEmail, subject, htmlBody, fromEmail, attachmentName, attachmentStream);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "The message could not be delivered.");
                throw;
            }
        }
    }
}