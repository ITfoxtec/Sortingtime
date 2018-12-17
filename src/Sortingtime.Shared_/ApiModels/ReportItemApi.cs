using Sortingtime.Models;
using System;

namespace Sortingtime.ApiModels
{
    public class ReportItemApi
    {
        public long Id { get; set; }

        public ReportStatus Status { get; set; }

        public long Number { get; set; }

        public int TotalTime { get; set; }

        public string ToEmail { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }

    }
}
