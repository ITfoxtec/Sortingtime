using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class LogoApi
    {
        [Required]
        public string Image { get; set; }
    }
}
