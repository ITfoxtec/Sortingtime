using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class TimeTaskApi
    {
		public long Id { get; set; }

        [Required]
        public long? GroupId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "The {0} field is an invalid date.")]
        public DateTime? Date { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Name { get; set; }

        public TimeTaskItemApi Item { get; set; }
    }
}