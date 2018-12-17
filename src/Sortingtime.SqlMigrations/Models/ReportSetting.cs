using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class ReportSetting
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
        public string ReportTitle { get; set; }

        [StringLength(4000)]
        public string ReportText { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }
    }
}
