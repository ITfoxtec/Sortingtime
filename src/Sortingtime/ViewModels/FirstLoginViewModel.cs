using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ViewModels
{
    public class FirstLoginViewModel
    {
		[Required(ErrorMessage = "The {0} is required")]
		[StringLength(200)]
		[EmailAddress(ErrorMessage = "The {0} is invalid")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "The {0} is required")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
		[DataType(DataType.Password)]
		[Display(Name = "Choose your Password")]
		public string Password { get; set; }

		public string Code { get; set; }
	}
}