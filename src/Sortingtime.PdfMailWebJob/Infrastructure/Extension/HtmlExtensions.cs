using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sortingtime.PdfMailWebJob.Infrastructure
{
    public static class HtmlExtensions
    {
        public async static Task<MemoryStream> ToHtmlStreamAddStyle(this IEnumerable<string> htmlList, IEnumerable<KeyValuePair<string, string>> styles)
        {
            var htmlStream = new MemoryStream();
            var htmlWrite = new StreamWriter(htmlStream, Encoding.UTF8);

            var html = string.Join("\n", htmlList);

            await htmlWrite.WriteAsync(AddHtmlStyle(html, styles));
            await htmlWrite.FlushAsync();
            htmlStream.Position = 0;
            return htmlStream;
        }

        public static string ToHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var bodySplit = html.Split('\n');
            return string.Join("<br />", bodySplit);
        }

        private static string AddHtmlStyle(string html, IEnumerable<KeyValuePair<string, string>> styles)
        {
            foreach (var style in styles)
            {
                html = html.Replace(string.Format("class='{0}'", style.Key), string.Format("style='{0}'", style.Value));
            }
            return html;
        }

        public static string ToEmailHtml(this string text)
        {
            return string.Join("", CreateEmailHtml(text));
        }

        private static IEnumerable<string> CreateEmailHtml(string body)
        {
            yield return "<html><body>";

            yield return body?.ToHtml();

            yield return "</body></html>";
        }
    }
}
