using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class HourPriceSettingApi
    {
        [StringLength(200)]
        public string GroupReferenceKey { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskReferenceKey { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public decimal HourPrice { get; set; }
    }
}
