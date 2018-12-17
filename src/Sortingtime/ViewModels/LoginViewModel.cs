using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "The {0} is required")]
		[StringLength(200)]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "The {0} is required")]
		[StringLength(100)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Keep me logged in")]
		public bool RememberMe { get; set; }
	}
}