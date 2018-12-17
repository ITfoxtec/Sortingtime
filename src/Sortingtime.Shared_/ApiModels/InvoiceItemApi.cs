using Sortingtime.Models;
using System;

namespace Sortingtime.ApiModels
{
    public class InvoiceItemApi
    {
        public long Id { get; set; }

        public InvoiceStatus Status { get; set; }

        public long Number { get; set; }

        public string CustomerShort { get; set; }        

        public DateTime InvoiceDate { get; set; }

        public decimal SubTotalPrice { get; set; }

        public string ToEmail { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }
    }
}
