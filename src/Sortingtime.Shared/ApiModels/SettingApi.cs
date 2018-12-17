using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class SettingApi
    {
        public class ReferenceTypes
        {
            public const string Group = "group";
            public const string Task = "task";
        }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReferenceType { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReferenceKey { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ToEmail { get; set; }

        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailSubject { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailBody { get; set; }
    }
}
