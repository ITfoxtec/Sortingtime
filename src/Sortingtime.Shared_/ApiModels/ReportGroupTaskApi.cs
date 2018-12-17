using System.Collections.Generic;

namespace Sortingtime.ApiModels
{
    public class ReportGroupTaskApi
    {
        public string Group { get; set; }

        public string Task { get; set; }

        public int MonthTotal { get; set; }

        public IEnumerable<ReportWorkApi> Works { get; set; }

    }
}
