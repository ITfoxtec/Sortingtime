using Sortingtime.ApiModels;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.PdfMailWebJob.Infrastructure;
using Sortingtime.PdfMailWebJob.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortingtime.PdfMailWebJob.HtmlGenerators
{
    public class ReportHtml
    {
        public async static Task<MemoryStream> CreateReportStream(Translate translate, bool showGroupColl, string organizationLogo, string organizationName, string organizationAddress, string reportTitle, string reportSubTitle, string reportText, ReportApi report)
        {
            var styles = new List<KeyValuePair<string, string>>();
            styles.AddRange(GenericCssStyles.GetDefaultStyles());
            styles.AddRange(GenericCssStyles.GetSortingtimeLogoStyles());
            styles.AddRange(GetReportHtmlStyles());

            return await CreatePdfReportHtml(translate, showGroupColl, organizationLogo, organizationName, organizationAddress, reportTitle, reportSubTitle, reportText, report).ToHtmlStreamAddStyle(styles);
        }

        private static IEnumerable<string> CreatePdfReportHtml(Translate translate, bool showGroupColl, string organizationLogo, string organizationName, string organizationAddress, string reportTitle, string reportSubTitle, string reportText, ReportApi report)
        {
            foreach (var style in GenericCssStyles.GetDefaultInlineStyles())
            {
                yield return style;
            }

            yield return "<html class='html'>";
            yield return "<body class='body'>";
            yield return "<div class='vertical-logo'>";
            yield return translate.Get("REPORT.POWERED_BY") + " <span class='logo-sorting'>Sorting</span><span class='logo-time'>time</span>";
            yield return "</div>";
            yield return "<div class='container'>";
            yield return "<div class='body-content'>";

            yield return "<div class='row'>";
            yield return "    <div class='col-md-3'>";
            yield return "        <div>";
            yield return "            <div class='report-logo'>" + organizationLogo != null ? "<img src='" + organizationLogo + "' />" : "" + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "    <div class='col-md-6'>";
            yield return "        <div>";
            yield return "            <div class='report-title'>" + reportTitle + "</div>";
            yield return "        </div>";
            yield return "        <div class='default-margin'>";
            yield return "            <div class='report-sub-title'>" + reportSubTitle + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "    <div class='col-md-3'>";
            yield return "        <div>";
            yield return "            <div class='organisation-name'>" + organizationName + "</div>";
            yield return "        </div>";
            yield return "        <div class='default-margin'>";
            yield return "            <div class='organisation-address'>" + organizationAddress?.ToHtml() + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "</div>";

            yield return "<div class='row'>";
            yield return "    <div class='col-md-12'>";
            yield return "        <div class='large-margin'>";
            yield return "            <div class='report-text'>" + reportText?.ToHtml() + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "</div>";

            yield return "<div class='row'>";
            yield return "<div class='col-md-12'>";
            yield return "    <table class='table'>";
            yield return "        <tbody>";
            yield return "            <tr>";
            if (showGroupColl)
            {
                yield return "            <th class='table-head'>" + translate.Get("REPORT.GROUP") + "</th>";
            }
            yield return "                <th class='table-head'>" + translate.Get("REPORT.TASK") + "</th>";
            yield return "                <th class='table-head-right'>" + translate.Get("REPORT.TIME") + (report.GroupTaskTotals.Count() > 1 ? " <small class='small'>(" + report.MonthTotal.ToTimeFormat() + ")</small>": "") +"</th>";
            yield return "            </tr>";

            foreach (var groupTaskTotal in report.GroupTaskTotals)
            {
                yield return "            <tr>";
                if (showGroupColl)
                {
                    yield return "            <td class='table-data'>" + groupTaskTotal.Group + "</td>";
                }
                yield return "                <td class='table-data'>" + groupTaskTotal.Task + "</td>";
                yield return "                <td class='table-data-right'>" + groupTaskTotal.MonthTotal.ToTimeFormat() + "</td>";
                yield return "            </tr>";
            }

            yield return "        </tbody>";

            yield return "    </table>";
            yield return "</div>";
            yield return "</div>";

            yield return "<div class='row'>";
            yield return "<div class='col-md-12'>";
            yield return "    <table class='table'>";
            yield return "        <tbody>";
            yield return "            <tr>";
            yield return "                <th class='table-head' colspan='" + (report.DaysInMonth + (showGroupColl ? 3 : 2)) + "'>" + translate.Get("REPORT.PERSON_TIME") + "</th>";
            yield return "            </tr>";

            foreach (var user in report.Users)
            {
                yield return "            <tr>";
                yield return "                <td class='table-data' colspan='" + (report.DaysInMonth + (showGroupColl ? 3 : 2)) + "'>" + user.FullName + (user.GroupTasks.Count() > 1 ? " <small class='small'>(" + user.MonthTotal.ToTimeFormat() + ")</small>" : "") + "</td>";
                yield return "            </tr>";
                yield return "            <tr class='sub-table'>";
                yield return "                <th class='sub-table-head-first' ></th>";
                if (showGroupColl)
                {
                    yield return "            <th class='sub-table-head-names1'>" + translate.Get("REPORT.GROUP") + "</th>";
                }
                yield return "                <th class='" + (showGroupColl ? "sub-table-head-names2" : "sub-table-head-names1") + "'>" + translate.Get("REPORT.TASK") + "</th>";
                for (int day = 1; day <= report.DaysInMonth; day++)
                {
                    yield return "            <th class='sub-table-head-numbers'>" + day + "</th>";
                }
                yield return "            </tr>";

                foreach (var groupTask in user.GroupTasks)
                {
                    yield return "            <tr class='sub-table'>";
                    yield return "                <td class='table-group-task-first'></td>";
                    if (showGroupColl)
                    {
                        yield return "            <td class='table-group-task-names1'>" + groupTask.Group + "</td>";
                    }
                    yield return "                <td class='" + (showGroupColl ? "table-group-task-names2" : "table-group-task-names1") + "'>" + groupTask.Task + " <small class='small'>(" + groupTask.MonthTotal.ToTimeFormat() + ")</small></td>";

                    for (int day = 1; day <= report.DaysInMonth; day++)
                    {
                        var currentWork = groupTask.Works.Where(w => w.Day == day).Select(w => w.Time).SingleOrDefault();
                        yield return "            <td class='table-group-task-time'>" + currentWork?.ToTimeFormat() + "</td>";
                    }
                    yield return "            </tr>";
                }
                yield return "            <tr>";
                yield return "                <td class='table-space-first'></td>";
                yield return "                <td class='table-space' colspan='" + (report.DaysInMonth + (showGroupColl ? 2 : 1)) + "'></td>";
                yield return "            </tr>";

            }

            yield return "        </tbody>";

            yield return "    </table>";
            yield return "</div>";
            yield return "</div>";

            yield return "</div>";
            yield return "</div>";
            yield return "</body></html>";
        }

        private static IEnumerable<KeyValuePair<string, string>> GetReportHtmlStyles()
        {         
            yield return new KeyValuePair<string, string>("report-title", "height: 50px; text-align: center; font-size: 24px; font-weight: bolder; padding-left: 15px; padding-right: 15px;");
            yield return new KeyValuePair<string, string>("report-sub-title", "text-align: center; padding-left: 15px; padding-right: 15px;");
            yield return new KeyValuePair<string, string>("report-text", "");
            yield return new KeyValuePair<string, string>("report-logo", "padding-right: 15px;");
            yield return new KeyValuePair<string, string>("organisation-name", "padding-left: 15px;");
            yield return new KeyValuePair<string, string>("organisation-address", "padding-left: 15px;");

            yield return new KeyValuePair<string, string>("table-head", "padding: 8px; vertical-align: top; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom;");
            yield return new KeyValuePair<string, string>("table-head-right", "padding: 8px; vertical-align: top; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: right; vertical-align: bottom;");
            yield return new KeyValuePair<string, string>("table-data", "padding: 8px; vertical-align: top; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-data-right", "padding: 8px; text-align: right; vertical-align: top; border-top: 1px solid #7ab4b0;");

            yield return new KeyValuePair<string, string>("table", "width: 100%; min-width: 100%; margin-bottom: 20px; border-collapse: collapse; border-spacing: 0;");
            yield return new KeyValuePair<string, string>("sub-table", "");
            yield return new KeyValuePair<string, string>("sub-table-head-first", "width: 0px; border-top: 0px; border-bottom: 0px; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 4px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("sub-table-head-names1", "background-color: #ededed; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("sub-table-head-names2", "background-color: #ededed; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 1px;");
            yield return new KeyValuePair<string, string>("sub-table-head-numbers", "text-align: center; background-color: #ededed; border-top: 0px; border-bottom: 2px solid #7ab4b0; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 3px; padding-bottom: 4px; padding-left: 3px; border-left: 1px solid #f8f8f8;");
            // width: 2.4%; ændret til flydende brede...

            yield return new KeyValuePair<string, string>("table-group-task-first", "width: 0px; border-top: 0px; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 4px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("table-group-task-names1", "background-color: #ededed; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 4px; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-group-task-names2", "background-color: #ededed; vertical-align: bottom; font-size: 11px; line-height: 1.3; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 1px; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-group-task-time", "text-align: right; font-size: 9px; background-color: #ededed; vertical-align: bottom; line-height: 1.3; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 1px; border-top: 1px solid #7ab4b0; border-left: 1px solid #f8f8f8;");

            yield return new KeyValuePair<string, string>("table-space-first", "border-top: 0px;");
            yield return new KeyValuePair<string, string>("table-space", "border-top: 0px; padding: 10px; height: 15px;");
        }

    }
}
