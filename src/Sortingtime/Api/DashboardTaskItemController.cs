using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Models;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sortingtime.Api.Controllers
{
    [Route("api/[controller]")]
    public class DashboardTaskItemController : SecureApiController
    {
        private readonly ILogger logger;
        public DashboardTaskItemController(ILogger<InvoicingController> logger, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.logger = logger;
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TimeTaskItemApi item)
        {
            await CleanWhiteSpace(item);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Only pass Date through
            item.Date = item.Date.Date;

            var taskItem = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.TaskId == item.TaskId && ti.Date == item.Date).FirstOrDefaultAsync();
            if (taskItem == null)
            {
                if (item.Time > 0)
                {
                    if (!(await DbContext.Tasks.Where(t => t.Group.PartitionId == CurrentPartitionId && t.Id == item.TaskId).AnyAsync()))
                    {
                        throw new NotFoundResultException($"Task (TaskId:{item.TaskId}) not found.");
                    }

                    taskItem = new TtaskItem { UserId = CurrentUserId, TaskId = item.TaskId.Value, Date = item.Date, Time = item.Time };
                    DbContext.TaskItems.Add(taskItem);
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                taskItem.Time = item.Time;
                await DbContext.SaveChangesAsync();
            }

            item.Id = taskItem?.Id;
            await CalculateTimeTotal(item, item.TaskId.Value, item.Date);
            return Json(item);
        }

        // PUT api/values/5
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patch(long id, [FromBody]TimeTaskItemApi item)
        {
            var taskItem = await DbContext.TaskItems.Where(w => w.UserId == CurrentUserId && w.Id == id).FirstOrDefaultAsync();
            if (taskItem == null)
            {
                throw new NotFoundResultException($"TaskItem (TaskItemId:{id}) not found.");
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, taskItem, includeForeignId: true);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }
            await DbContext.SaveChangesAsync();

            item.Id = id;
            await CalculateTimeTotal(item, taskItem.TaskId, taskItem.Date);
            return Json(item);
        }

        private async Task CalculateTimeTotal(TimeTaskItemApi item, long taskId, DateTime date)
        {
            var firstDayOfMonth = date.FirstDayOfMonth();
            var firstDayOfNextMonth = date.FirstDayOfNextMonth();
            var firstDayOfWeek = date.FirstDayOfWeek();
            var firstDayOfNextWeek = date.FirstDayOfNextWeek();

            item.WeekTotal = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.TaskId == taskId && ti.Date >= firstDayOfWeek && ti.Date < firstDayOfNextWeek).Select(ti => (int)ti.Time).SumAsync();
            item.MonthTotal = await DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.TaskId == taskId && ti.Date >= firstDayOfMonth && ti.Date < firstDayOfNextMonth).Select(ti => (int)ti.Time).SumAsync();
        }

    }
}
