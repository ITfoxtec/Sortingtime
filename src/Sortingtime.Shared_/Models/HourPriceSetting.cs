using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class HourPriceSetting
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [StringLength(200)]
        public string GroupReferenceKey { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskReferenceKey { get; set; }

        [Required]
        public decimal HourPrice { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }
    }
}
