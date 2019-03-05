using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sortingtime.ApiModels
{
    public class InvoiceAndContentApi
    {
        public bool? CreditNote { get; set; }

        public long? RelatedId { get; set; }

        [Required]
        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ToEmail { get; set; }

        [NotMapped]
        public string FromFullName { get; set; }

        [NotMapped]
        public string FromEmail { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailSubject { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailBody { get; set; }

        public bool showGroupColl { get; set; }

        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceTitle { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceCustomer { get; set; }

        [StringLength(50, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string VatNumber { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string PaymentDetails { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoicePaymentTerms { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }      

        [StringLength(100, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceReference { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceText { get; set; }

        public bool Tax { get; set; }

        public bool Vat { get; set; }

        public int? TaxPercentage { get; set; }

        public int? VatPercentage { get; set; }

        public decimal? TaxPrice { get; set; }

        public decimal? VatPrice { get; set; }

        public decimal TotalPrice { get; set; }

        [Required]
        public InvoiceApi Invoice { get; set; }
    }
}
