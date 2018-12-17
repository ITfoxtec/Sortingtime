using System.Collections.Generic;
using System.Net.Mail;

namespace Sortingtime.PdfMailWebJob.Infrastructure.Extension
{
    public static class StringExtensions
    {
        public static MailAddress[] ToMailAddressArray(this string value)
        {
            var emails = new List<MailAddress>();
            foreach (var mailItem in value.Split(','))
            {
                emails.Add(new MailAddress(mailItem));
            }
            return emails.ToArray();
        }

        public static string HtmlSpace(this string value)
        {
            return value?.Replace(" ", "&nbsp;");
        }
    }
}
