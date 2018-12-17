using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class TimeTaskItemApi
    {
		public long? Id { get; set; }

        [Required]
        public long? TaskId { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "The {0} field is an invalid date.")]
        public DateTime Date { get; set; }

        [Range(0, 1440, ErrorMessage = "The {0} field must be between 0 and 24 hours.")]
        public short Time { get; set; }

        public int WeekTotal { get; set; }
        public int MonthTotal { get; set; }
    }
}