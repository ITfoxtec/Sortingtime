using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Organization
    {
        // Partition.Id and Organization.Id is the same
        public long Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(400)]
        public string Address { get; set; }

        [StringLength(50)]
        public string VatNumber { get; set; }

        [StringLength(400)]
        public string PaymentDetails { get; set; }

        public int? TaxPercentage { get; set; }

        public int? VatPercentage { get; set; }

        public string Logo { get; set; }

        public int? FirstInvoiceNumber { get; set; }        

        [Required]
        [StringLength(10)]
        [Display(Name = "Culture")]
        public string Culture { get; set; }

        public virtual Partition Partition { get; set; }
    }
}
