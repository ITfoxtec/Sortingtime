using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sortingtime.ApiModels
{
    public class ReportAndContentApi
    {
        public long? RelatedId { get; set; }

        [Required]
        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ToEmail { get; set; }

        [NotMapped]
        public string FromFullName { get; set; }

        [NotMapped]
        public string FromEmail { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailSubject { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string EmailBody { get; set; }

        public bool showGroupColl { get; set; }
       
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReportTitle { get; set; }

        [StringLength(400, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReportSubTitle { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReportText { get; set; }

        [Required]
        public ReportApi Report { get; set; }
    }
}
