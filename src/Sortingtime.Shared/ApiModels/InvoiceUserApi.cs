namespace Sortingtime.ApiModels
{
    public class InvoiceUserApi
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public int Time { get; set; }

        public decimal? HourPrice { get; set; }

        public decimal? Price { get; set; }
    }
}
