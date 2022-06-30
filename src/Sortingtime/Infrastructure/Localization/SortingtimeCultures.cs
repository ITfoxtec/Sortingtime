using System;
using System.Globalization;
using System.Linq;

namespace Sortingtime.Infrastructure.Localization
{
    public static class SortingtimeCultures
    {
        private static string[] SupportedCultureNames { get; } = new string[] { "da-DK", "en-US", "en-GB", "en-BE" };

        public static CultureInfo[] SupportedCultures { get; } = SupportedCultureNames.Select(c => new CultureInfo(c)).ToArray(); 

        public static CultureInfo DefaultCulture { get; } = new CultureInfo("en-US");

        public static CultureInfo DkCulture { get; } = new CultureInfo("da-DK");

        public static bool IsSupported(string cultureName)
        {
            foreach (var supportedCultureName in SupportedCultureNames)
            {
                if(supportedCultureName == cultureName)
                {
                    return true;
                }
            }
            return false;
        }

        public static string CultureToFullCultureName()
        {
            var culture = CultureInfo.CurrentCulture;

            if (IsSupported(culture.Name))
            {
                return culture.Name;
            }
            else if(culture.Parent != null && DkCulture.Parent.Name.Equals(culture.Parent.Name, StringComparison.OrdinalIgnoreCase))
            { 
                return DkCulture.Name;
            }

            return DefaultCulture.Name;
        }
    }
}
