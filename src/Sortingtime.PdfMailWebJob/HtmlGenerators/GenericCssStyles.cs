using System.Collections.Generic;

namespace Sortingtime.PdfMailWebJob.HtmlGenerators
{
    public class GenericCssStyles
    {
        public static IEnumerable<string> GetDefaultInlineStyles()
        {
            yield return "<style>";

            yield return "* { box-sizing: border-box; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; }";

            yield return ".inner-row { margin-bottom: 5px; padding-left: 0px; padding-right: 0px; }";
            yield return ".inner-row:after { clear: both; }";
            yield return ".inner-row:before, .inner-row:after { content: ' '; display: table; }";

            yield return ".row { margin-bottom: 15px; padding-left: 0px; padding-right: 0px; }";
            yield return ".row:after { clear: both; }";
            yield return ".row:before, .row:after { content: ' '; display: table; }";

            yield return "</style>";

        }

        public static IEnumerable<KeyValuePair<string, string>> GetDefaultStyles()
        {
            var margienLeft = 25;
            var margienRight = 25;
            //var width = 815;

            yield return new KeyValuePair<string, string>("html", "");
            yield return new KeyValuePair<string, string>("body", "margin: 0px; font-family: Arial; font-size: 15px; line-height: 1.4285; color: #333333;");
            yield return new KeyValuePair<string, string>("small", "font-size: 80%;");
            yield return new KeyValuePair<string, string>("hr", "border-top: 1px solid #7ab4b0");
            yield return new KeyValuePair<string, string>("container", "margin-left: " + margienLeft + "px; margin-right: " + margienRight + "px; padding-left: 0px; padding-right: 0px;");
            yield return new KeyValuePair<string, string>("body-content", "padding-left: 0px; padding-right: 0px;");

            yield return new KeyValuePair<string, string>("control-label", ""); // text-align: right; margin-bottom: 0px; padding-top: 7px; display: inline-block;

            yield return new KeyValuePair<string, string>("col-md-3", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 25%;");
            yield return new KeyValuePair<string, string>("col-md-4", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 33.33%;");
            yield return new KeyValuePair<string, string>("col-md-offset-5-col-md-4", "margin-left: 41.66%; min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 33.33%;");
            yield return new KeyValuePair<string, string>("col-md-5", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 41.66%;");
            yield return new KeyValuePair<string, string>("col-md-offset-4-col-md-5", "margin-left: 33.33%; min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 41.66%;");
            yield return new KeyValuePair<string, string>("col-md-6", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 50%;");
            yield return new KeyValuePair<string, string>("col-md-offset-1-col-md-6", "margin-left: 8.33%; min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 50%;");
            yield return new KeyValuePair<string, string>("col-md-7", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 58.33%");
            yield return new KeyValuePair<string, string>("col-md-8", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 66.66%;");
            yield return new KeyValuePair<string, string>("col-md-12", "min-height: 1px; padding-left: 0px; padding-right: 0px; vertical-align: top; float: left; width: 100%;");

            yield return new KeyValuePair<string, string>("default-margin", "margin-top: 2px;");
            yield return new KeyValuePair<string, string>("large-margin", "margin-top: 15px; margin-bottom: 15px;");
        }

        public static IEnumerable<KeyValuePair<string, string>> GetSortingtimeLogoStyles()
        {
            yield return new KeyValuePair<string, string>("vertical-logo", "position: absolute; top: 1150px; left: -40px; float: left; font-size: 10px; -moz-transform: rotate(-270.0deg); -o-transform: rotate(-270.0deg); -webkit-transform: rotate(-270.0deg); filter: progid: DXImageTransform.Microsoft.BasicImage(rotation=0.083); -ms-filter: \"progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083)\"; transform: rotate(-90.0deg);");
            /* 270deg
               http://www.html-to-pdf.net/
                 -moz-transform: rotate(-270.0deg);
                 -o-transform: rotate(-270.0deg);
                  -webkit-transform: rotate(-270.0deg);
                  filter: progid: DXImageTransform.Microsoft.BasicImage(rotation=0.083);
                  -ms-filter: "progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083)";
                  transform: rotate(-90.0deg);

               Spire.PDF
                  writing-mode: tb-rl;
            */
            yield return new KeyValuePair<string, string>("logo-sorting", "color: #fc7e01; margin-left: 0px;");
            yield return new KeyValuePair<string, string>("logo-time", "color: #7ab4b0; margin-left: 0px;");
        }
    }
}
