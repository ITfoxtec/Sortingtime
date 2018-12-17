using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class DefaultTask
	{
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
		public long TaskId { get; set; }
		public virtual Ttask Task { get; set; }
	}
}