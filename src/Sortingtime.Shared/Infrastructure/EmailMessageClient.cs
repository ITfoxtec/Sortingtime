﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public class EmailMessageClient
    {
        public string DefaultFromEmail { get; set; }
        public string ApiKey { get; set; }

        public Action<string> MessageSendtAudit { get; set; }

        public async Task SendEmailAsync(MailAddress[] toEmail, string subject, string htmlBody, MailAddress fromEmail = null, string attachmentName = null, MemoryStream attachmentStream = null)
        {
            var mail = new SendGridMessage();

            if (fromEmail != null)
            {
                mail.From = new EmailAddress(fromEmail.Address);
            }
            else
            {
                mail.From = new EmailAddress(DefaultFromEmail);
            }

            if (toEmail != null && toEmail.Length > 0)
            {
                mail.AddTo(new EmailAddress(toEmail[0].Address));
                if (toEmail.Length > 1)
                {
                    for (int i = 1; i < toEmail.Length; i++)
                    {
                        mail.AddCc(new EmailAddress(toEmail[i].Address));
                    }
                }
            }

            mail.Subject = subject;
            mail.AddContent("text/html", htmlBody);

            if (!string.IsNullOrEmpty(attachmentName) && attachmentStream != null)
            {           
                mail.AddAttachment(attachmentName, Convert.ToBase64String(attachmentStream.ToArray()), "application/pdf");
            }

            var client = new SendGridClient(ApiKey);
            var response = await client.SendEmailAsync(mail);

            MessageSendtAudit($"Email Send [Status: {response.StatusCode}]. [{subject}] e-mail to addresses [{string.Join(", ", toEmail.Select(a => a.Address).ToArray())}].");

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Send email error. Status: {response.StatusCode}, to:[{string.Join(", ", toEmail.Select(a => a.Address).ToArray())}].");
            }
        }
    }
}
