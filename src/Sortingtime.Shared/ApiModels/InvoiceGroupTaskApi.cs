using System.Collections.Generic;

namespace Sortingtime.ApiModels
{
    public class InvoiceGroupTaskApi
    {
        public string Group { get; set; }

        public string Task { get; set; }

        public int Time { get; set; }

        public decimal? Price { get; set; }

        public IEnumerable<InvoiceUserApi> Users { get; set; }
    }
}
