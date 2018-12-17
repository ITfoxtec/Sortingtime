namespace Sortingtime.ApiModels
{
    public class ReportDataApi
    {
        public bool ShowGroupColl { get; set; }

        public string ReportTitle { get; set; }

        public string ReportText { get; set; }

        public string ReportSubTitle { get; set; }

        public ReportApi Report { get; set; }
    }
}