using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class InvoiceSettingApi : SettingApi
    {
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceTitle { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceCustomer { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoicePaymentTerms { get; set; }

        [StringLength(100, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceReference { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string InvoiceText { get; set; }
    }
}
