using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class DashboardItemApi
    {
        public char? State { get; set; }

        [Required]
        public char Type { get; set; }

        public string UniqId { get; set; }

        public long Id { get; set; }

        public long? GroupId { get; set; }
        public long? TaskItemId { get; set; }
        public long? MaterialItemId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "The {0} field is an invalid date.")]
        public DateTime? Date { get; set; }

        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Name { get; set; }

        [Range(0, 1440, ErrorMessage = "The {0} field must be between 0 and 24 hours.")]
        public short? Time { get; set; }

        public int? DayTaskTotal { get; set; }
        public int? WeekTaskTotal { get; set; }
        public int? MonthTaskTotal { get; set; }

        public decimal? Price { get; set; }

    }
}
