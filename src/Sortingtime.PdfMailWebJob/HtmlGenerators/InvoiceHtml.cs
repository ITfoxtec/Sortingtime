using Sortingtime.ApiModels;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
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
    public class InvoiceHtml
    {
        public async static Task<MemoryStream> CreateInvoiceStream(Translate translate, Invoice invoice, InvoiceDataApi invoiceData, string organizationLogo, string organizationName, string organizationAddress)
        {
            var styles = new List<KeyValuePair<string, string>>();
            styles.AddRange(GenericCssStyles.GetDefaultStyles());
            styles.AddRange(GenericCssStyles.GetSortingtimeLogoStyles());
            styles.AddRange(GetInvoiceHtmlStyles());          

            return await CreatePdfInvoiceHtml(translate, invoice, invoiceData, organizationLogo, organizationName, organizationAddress).ToHtmlStreamAddStyle(styles);
        }

        private static IEnumerable<string> CreatePdfInvoiceHtml(Translate translate, Invoice invoice, InvoiceDataApi invoiceData, string organizationLogo, string organizationName, string organizationAddress)
        {
            foreach(var style in GenericCssStyles.GetDefaultInlineStyles())
            {
                yield return style;
            }

            yield return "<html class='html'>";
            yield return "<body class='body'>";
            yield return "<div class='vertical-logo'>";
            yield return translate.Get("INVOICE.POWERED_BY") + " <span class='logo-sorting'>Sorting</span><span class='logo-time'>time</span>";
            yield return "</div>";
            yield return "<div class='container'>";
            yield return "<div class='body-content'>";

            yield return "<div class='row'>";
            yield return "    <div class='col-md-3'>";
            yield return "        <div>";
            yield return "            <div class='invoice-logo'>" + organizationLogo != null ? "<img src='" + organizationLogo + "' />" : "" + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "    <div class='col-md-6'>";
            yield return "        <div>";
            yield return "            <div class='invoice-title'>" + invoiceData.InvoiceTitle + "</div>";
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
            yield return "    <div class='col-md-3'>";
            yield return "        <div class='large-margin'>";
            yield return "            <div class='invoice-customer'>" + invoiceData.InvoiceCustomer?.ToHtml() + "</div>";
            yield return "        </div>";
            yield return "    </div>";
            yield return "    <div class='col-md-offset-5-col-md-4'>";
            if (invoiceData.Vat && !string.IsNullOrWhiteSpace(invoiceData.VatNumber))
            {
                yield return "      <div class='inner-row'>";
                yield return "          <div class='col-md-5'>";
                yield return "              <div>";
                yield return "                  <div class='control-label'>" + translate.Get("INVOICE.VAT_NUMBER") + ":</div>";
                yield return "              </div>";
                yield return "          </div>";
                yield return "          <div class='col-md-7'>";
                yield return "              <div>";
                yield return "                  <div class='vat-number'>" + invoiceData.VatNumber + "</div>";
                yield return "              </div>";
                yield return "          </div>";
                yield return "      </div>";
            }

            if(!string.IsNullOrWhiteSpace(invoiceData.PaymentDetails))
            {
                yield return "        <div class='large-margin'>";
                yield return "            <div class='payment-details'>" + invoiceData.PaymentDetails?.ToHtml() + "</div>";
                yield return "        </div>";
            }

            yield return "    </div>";
            yield return "</div>";

            yield return "<div class='row'>";
            yield return "    <div class='col-md-12'>";
            yield return "        <hr class='hr' />";
            yield return "    </div>";
            yield return "</div>";


            yield return "<div class='row'>";
            yield return "    <div class='col-md-5'>";
            yield return "      <div class='inner-row'>";
            yield return "          <div class='col-md-5'>";
            yield return "              <div>";
            yield return "                  <div class='control-label'>" + translate.Get("INVOICE.INVOICE_NUMBER") + ":</div>";
            yield return "              </div>";
            yield return "          </div>";
            yield return "          <div class='col-md-7'>";
            yield return "              <div>";
            yield return "                  <div class='invoice-number'>" + invoice.Number + "</div>";
            yield return "              </div>";
            yield return "          </div>";
            yield return "      </div>";
            yield return "      <div class='inner-row'>";
            yield return "          <div class='col-md-5'>";
            yield return "              <div>";
            yield return "                  <div class='control-label'>" + translate.Get("INVOICE.INVOICE_DATE") + ":</div>";
            yield return "              </div>";
            yield return "          </div>";
            yield return "          <div class='col-md-7'>";
            yield return "              <div>";
            yield return "                  <div class='invoice-date'>" + invoice.InvoiceDate.ToShortDateString() + "</div>";
            yield return "              </div>";
            yield return "          </div>";
            yield return "      </div>";
            if (!string.IsNullOrWhiteSpace(invoiceData.InvoiceReference))
            {
                yield return "      <div class='inner-row'>";
                yield return "          <div class='col-md-5'>";
                yield return "              <div>";
                yield return "                  <div class='control-label'>" + translate.Get("INVOICE.REFERENCE") + ":</div>";
                yield return "              </div>";
                yield return "          </div>";
                yield return "          <div class='col-md-7'>";
                yield return "              <div>";
                yield return "                  <div class='invoice-reference'>" + invoiceData.InvoiceReference + "</div>";
                yield return "              </div>";
                yield return "          </div>";
                yield return "      </div>";
            }
            yield return "    </div>";

            if (!string.IsNullOrWhiteSpace(invoiceData.InvoicePaymentTerms))
            {
                yield return "    <div class='col-md-offset-1-col-md-6'>";
                yield return "        <div class='large-margin'>";
                yield return "            <div class='payment-terms'>" + invoiceData.InvoicePaymentTerms?.ToHtml() + "</div>";
                yield return "        </div>";
                yield return "    </div>";
            }
            yield return "</div>";

            if (!string.IsNullOrWhiteSpace(invoiceData.InvoiceText))
            {
                yield return "<div class='row'>";
                yield return "    <div class='col-md-12'>";
                yield return "        <div class='large-margin'>";
                yield return "            <div class='invoice-text'>" + invoiceData.InvoiceText?.ToHtml() + "</div>";
                yield return "        </div>";
                yield return "    </div>";
                yield return "</div>";
            }
            yield return "<div class='row'>";
            yield return "<div class='col-md-12'>";
            yield return "    <table class='table'>";

            yield return "    <colgroup>";
            yield return "        <col style='width: 20px' />";
            if (invoiceData.ShowGroupColl)
            {
                yield return "        <col style='width: 25%' />";
            }
            yield return "        <col style='width: auto' />";
            yield return "        <col style='width: 15%' />";
            yield return "        <col style='width: 15%' />";
            yield return "        <col style='width: 18%' />";
            yield return "    </colgroup>";

            yield return "        <tbody>";
            yield return "            <tr>";
            if (invoiceData.ShowGroupColl)
            {
                yield return "            <th class='table-head' colspan='2'>" + translate.Get("INVOICE.GROUP") + "</th>";
            }
            yield return "                <th class='table-head' colspan='" + (invoiceData.ShowGroupColl ? "1" : "2") + "'>" + translate.Get("INVOICE.TASK") + "</th>";
            yield return "                <th class='table-head-right'>" + translate.Get("INVOICE.TIME") + "</th>";
            yield return "                <th class='table-head-right'>" + translate.Get("INVOICE.HOURPRICE") + "</th>";
            yield return "                <th class='table-head-right'>" + translate.Get("INVOICE.PRICE") + "</th>";
            yield return "            </tr>";

            foreach (var groupTask in invoiceData.Invoice.GroupTasks)
            {
                yield return "            <tr>";
                if (invoiceData.ShowGroupColl)
                {
                    yield return "            <td class='table-data' colspan='2'>" + groupTask.Group + "</td>";
                }
                yield return "                <td class='table-data' colspan='" + (invoiceData.ShowGroupColl ? "1" : "2") + "'>" + groupTask.Task + "</td>";
                yield return "                <td class='table-data'></td>";
                yield return "                <td class='table-data'></td>";
                yield return "                <td class='table-data'></td>";
                yield return "            </tr>";

                yield return "            <tr class='sub-table'>";
                yield return "                <th class='sub-table-head-first'></th>";
                yield return "                <th class='sub-table-head-names1' colspan='" + (invoiceData.ShowGroupColl ? "2" : "1") + "'>" + translate.Get("INVOICE.PERSON") + "</th>";
                yield return "                <th class='sub-table-head-names2'></th>";
                yield return "                <th class='sub-table-head-names2'></th>";
                yield return "                <th class='sub-table-head-names2'></th>";
                yield return "            </tr>";

                foreach (var user in groupTask.Users)
                {
                    yield return "            <tr class='sub-table'>";
                    yield return "                <td class='table-user-first'></td>";
                    yield return "                <td class='table-user-names1' colspan='" + (invoiceData.ShowGroupColl ? "2" : "1") + "'>" + user.FullName + "</td>";
                    yield return "                <td class='table-user-data-right'>" + user.Time.ToTimeFormat() + "</td>";
                    yield return "                <td class='table-user-data-right'>" + user.HourPrice?.ToString("C").HtmlSpace() + "</td>";
                    yield return "                <td class='table-user-data-right'>" + user.Price?.ToString("C").HtmlSpace() + "</td>";
                    yield return "            </tr>";
                }

                yield return "            <tr>";
                yield return "                <td class='table-space' colspan='" + (invoiceData.ShowGroupColl ? "6" : "5") + "'></td>";
                yield return "            </tr>";
            }

            yield return "            <tr>";
            yield return "                <td class='table-space-small' colspan='" + (invoiceData.ShowGroupColl ? "6" : "5") + "'></td>";
            yield return "            </tr>";

            yield return "            <tr>";
            yield return "                <td class='table-space' colspan='" + (invoiceData.ShowGroupColl ? "3" : "2") + "'></td>";
            yield return "                <td colspan='2' class='table-data'>" + translate.Get("INVOICE.SUBTOTAL") + "</td>";
            yield return "                <td class='table-data-right'>" + invoice.SubTotalPrice.ToString("C").HtmlSpace() + "</td>";
            yield return "            </tr>";

            if (invoiceData.Tax)
            {
                yield return "            <tr>";
                yield return "                <td class='table-space' colspan='" + (invoiceData.ShowGroupColl ? "3" : "2") + "'></td>";
                yield return "                <td colspan='2' class='table-data'>" + translate.Get("INVOICE.TAX") + " (" + (invoiceData.TaxPercentage.HasValue ? invoiceData.TaxPercentage.Value : 0) + "%)</td>";
                yield return "                <td class='table-data-right'>" + invoiceData.TaxPrice?.ToString("C").HtmlSpace() + "</td>";
                yield return "            </tr>";
            }
            if (invoiceData.Vat)
            {
                yield return "            <tr>";
                yield return "                <td class='table-space' colspan='" + (invoiceData.ShowGroupColl ? "3" : "2") + "'></td>";
                yield return "                <td colspan='2' class='table-data'>" + translate.Get("INVOICE.VAT") + " (" + (invoiceData.VatPercentage.HasValue ? invoiceData.VatPercentage.Value : 0) + "%)</td>";
                yield return "                <td class='table-data-right'>" + invoiceData.VatPrice?.ToString("C").HtmlSpace() + "</td>";
                yield return "            </tr>";
            }

            yield return "            <tr>";
            yield return "                <td class='table-space' colspan='" + (invoiceData.ShowGroupColl ? "3" : "2") + "'></td>";
            yield return "                <td colspan='2' class='table-data-total'>" + translate.Get("INVOICE.TOTAL") + "</td>";
            yield return "                <td class='table-data-total-right'>" + invoiceData.TotalPrice.ToString("C").HtmlSpace() + "</td>";
            yield return "            </tr>";
        

            yield return "        </tbody>";

            yield return "    </table>";
            yield return "</div>";
            yield return "</div>";

            yield return "</div>";
            yield return "</div>";
            yield return "</body></html>";
        }

        private static IEnumerable<KeyValuePair<string, string>> GetInvoiceHtmlStyles()
        {
            yield return new KeyValuePair<string, string>("invoice-title", "height: 50px; text-align: center; font-size: 24px; font-weight: bolder; padding-left: 15px; padding-right: 15px;");

            yield return new KeyValuePair<string, string>("invoice-logo", "padding-right: 15px;");
            yield return new KeyValuePair<string, string>("organisation-name", "padding-left: 15px;");
            yield return new KeyValuePair<string, string>("organisation-address", "padding-left: 15px;");

            yield return new KeyValuePair<string, string>("invoice-customer", "");
            yield return new KeyValuePair<string, string>("vat-number", "");
            yield return new KeyValuePair<string, string>("payment-details", "");
            yield return new KeyValuePair<string, string>("payment-terms", "");
            yield return new KeyValuePair<string, string>("invoice-number", "");
            yield return new KeyValuePair<string, string>("invoice-date", "");
            yield return new KeyValuePair<string, string>("invoice-reference", "");
            yield return new KeyValuePair<string, string>("invoice-text", "");

            yield return new KeyValuePair<string, string>("table", "table-layout:fixed; width: 100%; min-width: 100%; margin-bottom: 20px; border-collapse: collapse; border-spacing: 0;");
            yield return new KeyValuePair<string, string>("sub-table", "");
            yield return new KeyValuePair<string, string>("table-head", "padding: 8px; vertical-align: top; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom;");
            yield return new KeyValuePair<string, string>("table-head-right", "padding: 8px; vertical-align: top; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: right; vertical-align: bottom;");
            yield return new KeyValuePair<string, string>("table-data", "padding: 8px; vertical-align: top; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-data-right", "padding: 8px; text-align: right; vertical-align: top; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-data-total", "font-weight: bolder; padding: 8px; vertical-align: top; border-top: 1px solid #7ab4b0; border-bottom: 2px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-data-total-right", "font-weight: bolder; padding: 8px; text-align: right; vertical-align: top; border-top: 1px solid #7ab4b0; border-bottom: 2px solid #7ab4b0;");

            yield return new KeyValuePair<string, string>("sub-table-head-first", "width: 20px; border-top: 0px; border-bottom: 0px; vertical-align: bottom; font-size: 12.5px; line-height: 1.32; padding-top: 4px; padding-right: 4px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("sub-table-head-names1", "background-color: #ededed; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom; font-size: 12.5px; line-height: 1.32; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("sub-table-head-names2", "background-color: #ededed; border-top: 0px; border-bottom: 2px solid #7ab4b0; text-align: left; vertical-align: bottom; font-size: 12.5px; line-height: 1.32; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 1px;");

            yield return new KeyValuePair<string, string>("table-user-first", "width: 20px; border-top: 0px; vertical-align: bottom; font-size: 12.5px; line-height: 1.32; padding-top: 4px; padding-right: 4px; padding-bottom: 4px; padding-left: 4px;");
            yield return new KeyValuePair<string, string>("table-user-names1", "background-color: #ededed; vertical-align: bottom; font-size: 12.5px; line-height: 1.32; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 4px; border-top: 1px solid #7ab4b0;");
            yield return new KeyValuePair<string, string>("table-user-data-right", "text-align: center; font-size: 12.5px; background-color: #ededed; text-align: right; vertical-align: bottom; line-height: 1.32; padding-top: 4px; padding-right: 1px; padding-bottom: 4px; padding-left: 1px; border-top: 1px solid #7ab4b0;");
            
            yield return new KeyValuePair<string, string>("table-space", "border-top: 0px; padding: 10px; height: 15px;");
            yield return new KeyValuePair<string, string>("table-space-small", "border-top: 0px; padding: 10px; height: 7px;");

        }
    }
}
