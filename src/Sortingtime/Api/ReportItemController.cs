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
    public class ReportItemController : SecureApiController
    {
        public ReportItemController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                if (!fromDate.HasValue)
                    ModelState.AddModelError("fromDate", "Required Date is null or empty.");
                if (!toDate.HasValue)
                    ModelState.AddModelError("toDate", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }

            var fromDateValue = fromDate.Value;
            var toDateValue = toDate.Value;

            var reports = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.Status != ReportStatus.Deleted && r.ToDate >= fromDateValue && r.ToDate <= toDateValue)
                .Select(r => new ReportItemApi
                {
                    Id = r.Id,
                    Status = r.Status,
                    Number = r.Number,
                    TotalTime = r.TotalTime,
                    ToEmail = r.ToEmail,
                }).ToListAsync();
                
            return Json(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var report = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.Status != ReportStatus.Deleted && r.Id == id)
                .Select(r => new ReportItemApi
                {
                    Id = r.Id,
                    Status = r.Status,
                    Number = r.Number,
                    TotalTime = r.TotalTime,
                    ToEmail = r.ToEmail,
                    EmailSubject = r.EmailSubject,
                    EmailBody = r.EmailBody,
                }).FirstOrDefaultAsync();

            if (report == null)
            {
                return new NotFoundResult();
            }
            return Json(report);
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var report = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.Id == id).FirstOrDefaultAsync();
            if (report == null)
            {
                return new NotFoundResult();
            }

            report.Status = ReportStatus.Deleted;
            await DbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
