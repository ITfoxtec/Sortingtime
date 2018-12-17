using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortingtime.Models
{
    public class Invoice
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdateTimestamp { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; }

        [Required]
        public long Number { get; set; }

        public string CustomerShort { get; set; }

        [StringLength(400)]
        public string ToEmail { get; set; }

        [StringLength(200)]
        public string FromFullName { get; set; }

        [StringLength(200)]
        public string FromEmail { get; set; }

        [StringLength(400)]
        public string EmailSubject { get; set; }

        [StringLength(4000)]
        public string EmailBody { get; set; }

        public decimal SubTotalPrice { get; set; }

        public string InvoiceData { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public static Invoice CreateNew(long partitionId, long userId, long number)
        {
            return new Invoice
            {
                Timestamp = DateTime.UtcNow,
                Status = InvoiceStatus.Created,
                Number = number,
                PartitionId = partitionId,
                UserId = userId                
            };
        }

    }
}
