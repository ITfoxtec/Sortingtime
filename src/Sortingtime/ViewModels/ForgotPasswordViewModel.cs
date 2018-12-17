using System;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "The {0} is required")]
		[StringLength(200)]
		[EmailAddress(ErrorMessage = "The {0} is invalid")]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}