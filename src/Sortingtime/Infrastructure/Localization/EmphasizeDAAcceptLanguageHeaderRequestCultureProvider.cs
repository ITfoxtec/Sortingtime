using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Net.Http.Headers;

namespace Sortingtime.Infrastructure.Localization
{
    public class EmphasizeDAAcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
    {
        /// <summary>
        /// The maximum number of values in the Accept-Language header to attempt to create a <see cref="System.Globalization.CultureInfo"/>
        /// from for the current request.
        /// Defaults to <c>3</c>.
        /// </summary>
        public int MaximumAcceptLanguageHeaderValuesToTry { get; set; } = 3;

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

            if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var languages = acceptLanguageHeader.AsEnumerable();

            if (MaximumAcceptLanguageHeaderValuesToTry > 0)
            {
                // We take only the first configured number of languages from the header and then order those that we
                // attempt to parse as a CultureInfo to mitigate potentially spinning CPU on lots of parse attempts.
                languages = languages.Take(MaximumAcceptLanguageHeaderValuesToTry);
            }

            var orderedLanguages = languages.OrderByDescending(h => h, StringWithQualityHeaderValueComparer.QualityComparer)
                .Select(x => x.Value).ToList();

            //foreach (var item in languages)
            //{
            //    System.Diagnostics.Debug.WriteLine($"acceptLanguageHeader; value: '{item?.Value}', Quality: '{item?.Quality}'");
            //    //if (item != null && item.Value != null && item.Value.IndexOf("da", StringComparison.InvariantCultureIgnoreCase) > -1)
            //    //{
            //    //    return await Task.FromResult(new ProviderCultureResult("da-DK"));
            //    //}
            //}

            if (orderedLanguages.Count(l => l.Equals(SortingtimeCultures.DkCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                return Task.FromResult(new ProviderCultureResult(SortingtimeCultures.DkCulture.Name));
            }

            if (orderedLanguages.Any())
            {
                return Task.FromResult(new ProviderCultureResult(orderedLanguages));
            }

            return Task.FromResult((ProviderCultureResult)null);
        }
    }
}


    //    : AcceptLanguageHeaderRequestCultureProvider
    //{
    //    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    //    {
    //        if (httpContext == null)
    //        {
    //            throw new ArgumentNullException(nameof(httpContext));
    //        }

    //        //var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

    //        //if (acceptLanguageHeader != null || acceptLanguageHeader.Count > 0)
    //        //{
    //        //    //foreach (var item in acceptLanguageHeader)
    //        //    //{
    //        //    //    System.Diagnostics.Debug.WriteLine($"acceptLanguageHeader; value: '{item?.Value}', Quality: '{item?.Quality}'");
    //        //    //    //if (item != null && item.Value != null && item.Value.IndexOf("da", StringComparison.InvariantCultureIgnoreCase) > -1)
    //        //    //    //{
    //        //    //    //    return await Task.FromResult(new ProviderCultureResult("da-DK"));
    //        //    //    //}
    //        //    //}
    //        //}

    //        return await base.DetermineProviderCultureResult(httpContext);
    //    }
    //}

