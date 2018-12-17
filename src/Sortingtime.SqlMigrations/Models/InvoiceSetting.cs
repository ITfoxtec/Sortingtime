using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class InvoiceSetting
    {
        public long Id { get; set; }

        [Required]
        public int ReferenceType { get; set; }

        [Required]
        [StringLength(200)]
        public string ReferenceKey { get; set; }

        [StringLength(400)]
        public string ToEmail { get; set; }

        [StringLength(400)]
        public string EmailSubject { get; set; }

        [StringLength(4000)]
        public string EmailBody { get; set; }

        [StringLength(200)]
        public string InvoiceTitle { get; set; }

        [StringLength(400)]
        public string InvoiceCustomer { get; set; }

        [StringLength(400)]
        public string InvoicePaymentTerms { get; set; }

        [StringLength(100)]
        public string InvoiceReference { get; set; }

        [StringLength(4000)]
        public string InvoiceText { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }
    }
}
