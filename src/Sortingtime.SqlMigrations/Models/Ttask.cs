using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sortingtime.Models
{
    [Table("Tasks")]
    public class Ttask
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public long GroupId { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<TtaskItem> Items { get; set; }

        public virtual ICollection<DefaultTask> Defaults { get; set; }
    }
}