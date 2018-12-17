using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class ReportSettingController : SecureApiController
    {
        public ReportSettingController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string referenceType, string referenceKey)
        {
            var dbReferenceType = 0;
            if (SettingApi.ReferenceTypes.Group.Equals(referenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 10;
            }
            else if (SettingApi.ReferenceTypes.Task.Equals(referenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 20;
            }
            else
            {
                ModelState.AddModelError(referenceType, "Invalid Report Settings Reference Type.");
                return new BadRequestObjectResult(ModelState);
            }

            var reportSetting = await DbContext.ReportSettings.Where(s => s.PartitionId == CurrentPartitionId && s.ReferenceType == dbReferenceType && s.ReferenceKey == referenceKey)
                .Select(s => new ReportSettingApi
                {
                    ReferenceType = referenceType,
                    ReferenceKey = referenceKey,
                    ToEmail = s.ToEmail,
                    EmailSubject = s.EmailSubject,
                    EmailBody = s.EmailBody,
                    ReportTitle = s.ReportTitle,
                    ReportText = s.ReportText,
                }).FirstOrDefaultAsync();

            if(reportSetting == null)
            {
                reportSetting = new ReportSettingApi
                {
                    ReferenceType = referenceType,
                    ReferenceKey = referenceKey,
                };
            }
            return Json(reportSetting);
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]ReportSettingApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var dbReferenceType = 0;
            if (SettingApi.ReferenceTypes.Group.Equals(item.ReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 10;
            }
            else if (SettingApi.ReferenceTypes.Task.Equals(item.ReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 20;
            }
            else
            {
                ModelState.AddModelError("item.ReferenceType", "Invalid Report Settings Reference Type.");
                return new BadRequestObjectResult(ModelState);
            }


            var reportSetting = await DbContext.ReportSettings.Where(s => s.PartitionId == CurrentPartitionId && s.ReferenceType == dbReferenceType && s.ReferenceKey == item.ReferenceKey).FirstOrDefaultAsync();
            if(reportSetting == null)
            {
                reportSetting = new ReportSetting
                {
                    PartitionId = CurrentPartitionId,
                    ReferenceType = dbReferenceType,
                    ReferenceKey = item.ReferenceKey,
                };
                DbContext.ReportSettings.Add(reportSetting);
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, reportSetting, new string[] { "ReferenceType", "ReferenceKey" }, updateNulls:true);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
