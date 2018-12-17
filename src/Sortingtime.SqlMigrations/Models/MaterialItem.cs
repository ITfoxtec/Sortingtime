using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class MaterialItem
    {
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int Quantity { get; set; }

		[Required]
		public long MaterialId { get; set; }
		public virtual Material Material { get; set; }

	}
}