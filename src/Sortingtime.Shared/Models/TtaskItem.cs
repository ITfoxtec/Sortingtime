using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sortingtime.Models
{
    [Table("TaskItems")]
    public class TtaskItem
    {
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public short Time { get; set; }

		[Required]
		public long TaskId { get; set; }
		public virtual Ttask Task { get; set; }

	}
}