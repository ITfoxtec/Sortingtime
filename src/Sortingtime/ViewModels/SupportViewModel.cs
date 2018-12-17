using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ViewModels
{
    public class SupportViewModel
    {
        [Required(ErrorMessage = "The {0} is required")]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "The {0} is required")]
        [StringLength(200)]
        [EmailAddress(ErrorMessage = "The {0} is invalid")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The {0} is required")]
        [StringLength(4000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Message")]
        public string Message { get; set; }

    }
}
