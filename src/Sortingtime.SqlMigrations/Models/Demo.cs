using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Demo
    {
        public long Id { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [StringLength(200)]
        public string RemoteIpAddress { get; set; }

        public static Demo CreateNew()
        {
            return new Demo
            {
                Timestamp = DateTime.UtcNow,
            };
        }
    }
}
