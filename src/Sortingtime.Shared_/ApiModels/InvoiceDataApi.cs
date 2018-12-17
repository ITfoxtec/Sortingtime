namespace Sortingtime.ApiModels
{
    public class InvoiceDataApi
    {
        public bool ShowGroupColl { get; set; }

        public string InvoiceTitle { get; set; }

        public string InvoiceCustomer { get; set; }

        public string VatNumber { get; set; }

        public string PaymentDetails { get; set; }

        public string InvoicePaymentTerms { get; set; }

        public string InvoiceReference { get; set; }

        public string InvoiceText { get; set; }

        public bool Tax { get; set; }

        public bool Vat { get; set; }

        public int? TaxPercentage { get; set; }

        public int? VatPercentage { get; set; }

        public decimal? TaxPrice { get; set; }

        public decimal? VatPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public InvoiceApi Invoice { get; set; }
    }
}
