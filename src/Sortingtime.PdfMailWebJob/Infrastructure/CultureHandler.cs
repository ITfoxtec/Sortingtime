using System.Globalization;
using System.Threading;

namespace Sortingtime.PdfMailWebJob.Infrastructure
{
    public class CultureHandler
    {
        public static void SetCulture(string cultureName)
        {
            var cultureInfo = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }
    }
}
