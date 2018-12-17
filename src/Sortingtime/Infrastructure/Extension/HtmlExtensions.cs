using System.Collections.Generic;
using System.IO;

namespace Sortingtime.Infrastructure
{
    public static class HtmlExtensions
    {
        public static string ToHtmlStringAddStyle(this IEnumerable<string> htmlList, IEnumerable<KeyValuePair<string, string>> styles)
        {
            var html = string.Join("\n", htmlList);
            return AddHtmlStyle(html, styles);
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

            //var match = Regex.Match(html, @"class='(?<className>[\w_-]+)'", RegexOptions.IgnoreCase);
            //if (match.Success && match.Groups["className"] != null && match.Groups["className"].Success)
            //{
            //    throw new Exception(string.Format("The report html kontain key [{0}] which is not replaced", match.Groups["className"].Value));
            //}

            return html;
        } 
    }
}
