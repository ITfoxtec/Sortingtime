using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class DefaultMaterial
    {
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
		public long MaterialId { get; set; }
		public virtual Material Material { get; set; }
	}
}