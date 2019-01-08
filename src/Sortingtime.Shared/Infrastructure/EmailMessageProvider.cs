using Microsoft.Extensions.Logging;
using Sortingtime.Models;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public class EmailMessageProvider
    {
        private readonly ILogger logger;
        private readonly MailSettings mailSettings;
        private readonly EmailMessageClient emailMessageClient;

        public EmailMessageProvider(ILogger<EmailMessageProvider> logger, MailSettings mailSettings)
        {
            this.logger = logger;
            this.mailSettings = mailSettings;
            emailMessageClient = new EmailMessageClient
            {
                DefaultFromEmail = mailSettings.DefaultFromEmail,
                ApiKey = mailSettings.ApiKey,
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