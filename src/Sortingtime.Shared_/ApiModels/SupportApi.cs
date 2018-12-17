using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class SupportApi
    {
        [StringLength(10000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string Message { get; set; }
    }
}
