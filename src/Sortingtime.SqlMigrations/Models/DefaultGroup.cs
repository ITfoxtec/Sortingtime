using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class DefaultGroup
    {
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
		public long GroupId { get; set; }
		public virtual Group Group { get; set; }
	}
}