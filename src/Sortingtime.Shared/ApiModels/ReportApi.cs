using System;
using System.Collections.Generic;

namespace Sortingtime.ApiModels
{
    public class ReportApi
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int DaysInMonth { get; set; }

        public IEnumerable<ReportGroupTaskApi> GroupTaskTotals { get; set; }

        public int MonthTotal { get; set; }

        public IEnumerable<ReportUserApi> Users { get; set; }
    }
}
