using System.ComponentModel.DataAnnotations;

namespace Sortingtime.ApiModels
{
    public class ReportSettingApi : SettingApi
    {
        [StringLength(200, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReportTitle { get; set; }

        [StringLength(4000, ErrorMessage = "The {0} field must be a maximum length of {1} characters.")]
        public string ReportText { get; set; }
    }
}
