using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class DashboardTotalController : SecureApiController
    {
        public DashboardTotalController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(DateTime date, int? groupId)
        {
            // Only pass Date through
            date = date.Date;

            var firstDayOfMonth = date.FirstDayOfMonth();
            var firstDayOfNextMonth = date.FirstDayOfNextMonth();
            var firstDayOfWeek = date.FirstDayOfWeek();
            var firstDayOfNextWeek = date.FirstDayOfNextWeek();

            var timeTotal = new TimeTotalApi();
            timeTotal.DayTotal = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Date == date && (!groupId.HasValue || ti.Task.GroupId == groupId.Value)).Select(ti => (int)ti.Time).SumAsync();
            timeTotal.WeekTotal = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Date >= firstDayOfWeek && ti.Date < firstDayOfNextWeek && (!groupId.HasValue || ti.Task.GroupId == groupId.Value)).Select(ti => (int)ti.Time).SumAsync();
            timeTotal.MonthTotal = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Date >= firstDayOfMonth && ti.Date < firstDayOfNextMonth && (!groupId.HasValue || ti.Task.GroupId == groupId.Value)).Select(ti => (int)ti.Time).SumAsync();

            return Json(timeTotal);

        }
    }
}
