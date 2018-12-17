using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class TimeMaterialItemApi
    {
		public long Id { get; set; }

        [Required]
        public long? TaskId { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "The {0} field is an invalid date.")]
        public DateTime Date { get; set; }

        public int Quantity { get; set; }

        public int WeekTotal { get; set; }

        public int MonthTotal { get; set; }
    }
}