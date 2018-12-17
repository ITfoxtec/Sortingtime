using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Material
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public long GroupId { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<MaterialItem> Items { get; set; }

        public virtual ICollection<DefaultMaterial> Defaults { get; set; }
    }
}