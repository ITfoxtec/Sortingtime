using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Log
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [Required]
        public LogSeverity Severity { get; set; }

        [StringLength(200)]
        public string LogSource { get; set; }

        public string Message { get; set; }
    }
}
