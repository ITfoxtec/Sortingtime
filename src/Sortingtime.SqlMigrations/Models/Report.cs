using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Report
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdateTimestamp { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required]
        public ReportStatus Status { get; set; }

        [Required]
        public long Number { get; set; }

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

        public int TotalTime { get; set; }

        public string ReportData { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public static Report CreateNew(long partitionId, long userId, long number)
        {
            return new Report
            {
                Timestamp = DateTime.UtcNow,
                Status = ReportStatus.Created,
                Number = number,
                PartitionId = partitionId,
                UserId = userId
            };
        }

    }
}
