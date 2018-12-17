using JSNLog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Sortingtime.Infrastructure.Localization;

namespace Sortingtime.Infrastructure.Configuration
{
    public static class SortingtimeApplicationBuilderExtensions
    {
        // Kopieret fra https://github.com/mperdeck/jsnlog/blob/aa3b186a36de49e930459898549afa225af7962c/src/JSNLog/PublicFacing/AspNet5/Configuration/Middleware/ApplicationBuilderExtensions.cs
        public static void UseSecureJSNLog(this IApplicationBuilder builder, ILoggerFactory loggerFactory)
        {
            var jsnlogConfiguration = new JsnlogConfiguration(); // See jsnlog.com/Documentation/Configuration
            builder.UseJSNLog(new LoggingAdapter(loggerFactory), jsnlogConfiguration);
        }

        public static void UseLocalization(this IApplicationBuilder builder)
        {
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                RequestCultureProviders = new IRequestCultureProvider[] { new UserSavedCultureProvider(), new QueryStringRequestCultureProvider(), new EmphasizeDAAcceptLanguageHeaderRequestCultureProvider() },
                SupportedCultures = SortingtimeCultures.SupportedCultures,
                SupportedUICultures = SortingtimeCultures.SupportedCultures,
                DefaultRequestCulture = new RequestCulture(SortingtimeCultures.DefaultCulture),                
            };
            builder.UseRequestLocalization(requestLocalizationOptions);
        }
    }
}
