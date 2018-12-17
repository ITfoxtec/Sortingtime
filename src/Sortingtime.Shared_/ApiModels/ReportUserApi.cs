using System.Collections.Generic;

namespace Sortingtime.ApiModels
{
    public class ReportUserApi
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public int MonthTotal { get; set; }

        public IEnumerable<ReportGroupTaskApi> GroupTasks { get; set; }
    }
}
