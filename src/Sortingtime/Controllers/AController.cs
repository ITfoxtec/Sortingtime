using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sortingtime.Controllers
{
    public class AController : SecurePageController
    {
        private readonly TelemetryClient telemetryClient;

        public AController(TelemetryClient telemetryClient, ApplicationDbContext dbContext) : base(dbContext)
		{
            this.telemetryClient = telemetryClient;
        }

        public IActionResult Index()
        {
            telemetryClient.TrackEvent("StartingSortingtimeApp", new Dictionary<string, string> { { "UserId", Convert.ToString(CurrentUserId) }, { "PartitionId", CurrentPartitionIdAsString } });

            ViewBag.SinglePage = true;
            ViewBag.Translate = new Translate();
            return View();
        }

        public IActionResult Terms()
        {
            ViewBag.Translate = new Translate();
            return View();
        }

        public IActionResult LicensingTerms()
        {
            ViewBag.Translate = new Translate();
            return View();
        }
    }
}
