using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public static class CalenderExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime from)
        {
            var startDate = from.Date;
            while (startDate.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
            {
                startDate = startDate.AddDays(-1);
            }
            return startDate;
        }

        public static DateTime FirstDayOfNextWeek(this DateTime from)
        {
            var startDate = from.Date.FirstDayOfWeek();
            return startDate.AddDays(7);
        }

        public static DateTime FirstDayOfMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1).AddMonths(1);
        }

        public static int DaysInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }
            
    }
}
