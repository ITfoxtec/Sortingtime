using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sortingtime.Infrastructure.Translation;
using System.Linq;

namespace Sortingtime.Infrastructure.Localization
{
    public static class HtmlTranslateHelper
    {
        public static string CultureName<T>(this IHtmlHelper<T> htmlHelper)
        {
            return Translate.CultureName?.ToLower();
        }

        public static string ParentCultureName<T>(this IHtmlHelper<T> htmlHelper)
        {
            return Translate.ParentCultureName?.ToLower();
        }

        public static bool IsParentCultureName<T>(this IHtmlHelper<T> htmlHelper, string cultureName)
        {
            if (Translate.ParentCultureName.Equals(cultureName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Trans<T>(this IHtmlHelper<T> htmlHelper, string key)
        {
            var translate = htmlHelper.ViewBag.Translate as Translate;
            if (translate == null)
            {
                translate = new Translate();
                htmlHelper.ViewBag.Translate = translate;
            }

            return translate.Get(key);
        }

        public static object RouteCulture(this HttpRequest httpRequest)
        {
            var culture = httpRequest.Query["culture"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return new { culture = culture };
        }

        public static object RouteCulture<T>(this IHtmlHelper<T> htmlHelper)
        {
            var culture = htmlHelper.ViewContext.HttpContext.Request.Query["culture"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(culture))
            {
                return null;
            }

            return new { culture = culture };
        }

        public static object RouteDkCultureForLogout<T>(this IHtmlHelper<T> htmlHelper)
        {
            var parentCultureName = string.IsNullOrEmpty(Translate.ParentCultureName) ? Translate.CultureName : Translate.ParentCultureName;

            if (SortingtimeCultures.DkCulture.Parent.Name.Equals(parentCultureName, System.StringComparison.OrdinalIgnoreCase))
            {
                return new { culture = SortingtimeCultures.DkCulture.Name };
                
            }

            return null;
        }
    }
}
