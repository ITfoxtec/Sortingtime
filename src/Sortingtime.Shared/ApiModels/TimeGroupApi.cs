    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class TimeGroupApi
    {
		public long Id { get; set; }

        [DataType(DataType.Date, ErrorMessage = "The {0} field is an invalid date.")]
        public DateTime? Date { get; set; }

        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Name { get; set; }

        public int DayTaskTotal { get; set; }
        public int WeekTaskTotal { get; set; }
        public int MonthTaskTotal { get; set; }

        public IEnumerable<TimeTaskApi> Tasks { get; set; }
        public IEnumerable<TimeMaterialApi> Materials { get; set; }        
    }
}