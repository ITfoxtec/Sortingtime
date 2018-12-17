using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class UserApi
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "The {0} field is not a valid email")]
        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} field must be at least {2} characters long.", MinimumLength = 5)]
        public string Password { get; set; }
    }
}