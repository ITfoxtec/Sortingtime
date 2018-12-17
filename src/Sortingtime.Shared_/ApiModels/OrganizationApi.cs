using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class OrganizationApi
    {
        public bool IsDemo { get; set; }

        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Name { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string VatNumber { get; set; }
        
        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string PaymentDetails { get; set; }

        public int? TaxPercentage { get; set; }

        public int? VatPercentage { get; set; }

        public int? FirstInvoiceNumber { get; set; }

        [StringLength(10, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Culture { get; set; }

        public int? UserCount { get; set; }
    }
}
