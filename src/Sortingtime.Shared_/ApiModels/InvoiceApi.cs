using System;
using System.Collections.Generic;

namespace Sortingtime.ApiModels
{
    public class InvoiceApi
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? TotalTime { get; set; }
        
        public decimal SubTotalPrice { get; set; }

        public IEnumerable<InvoiceGroupTaskApi> GroupTasks { get; set; }
    }
}
