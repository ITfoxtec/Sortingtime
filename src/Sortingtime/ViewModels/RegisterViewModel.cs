using Sortingtime.Models;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ViewModels
{
	public class RegisterViewModel
	{
        [Required]
        public Plans Plan { get; set; } = Plans.Free;

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
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}	
}