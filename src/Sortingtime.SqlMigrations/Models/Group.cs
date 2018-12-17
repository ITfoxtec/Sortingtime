using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Group
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public long PartitionId { get; set; }
        public virtual Partition Partition { get; set; }

        public virtual ICollection<Ttask> Tasks { get; set; }
        public virtual ICollection<Material> Materials { get; set; }

        public virtual ICollection<DefaultGroup> Defaults { get; set; }
    }
}
