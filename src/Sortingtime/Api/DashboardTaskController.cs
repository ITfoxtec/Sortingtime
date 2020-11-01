using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Models;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Sortingtime.Api.Controllers
{
    [Route("api/[controller]")]
    public class DashboardTaskController : SecureApiController
    {
        private readonly ILogger logger;
        public DashboardTaskController(ILogger<InvoicingController> logger, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.logger = logger;
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TimeTaskApi item)
        {
            await CleanWhiteSpace(item);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var task = await DbContext.Tasks.Where(t => t.Group.PartitionId == CurrentPartitionId && t.GroupId == item.GroupId && t.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
            if (task == null)
            {
                if (!(await DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId && g.Id == item.GroupId).AnyAsync()))
                {
                    throw new NotFoundResultException($"Group (GroupId:{item.GroupId}) not found.");
                }

                task = new Ttask { GroupId = item.GroupId.Value, Name = item.Name };
                DbContext.Tasks.Add(task);

                if (item.Date.HasValue && item.Date.Value.Date == DateTime.Now.Date)
                {
                    DbContext.DefaultTasks.Add(new DefaultTask { UserId = CurrentUserId, Task = task });
                }

                await DbContext.SaveChangesAsync();
            }
            else
            {
                await MapDeltaValuesAndCleanWhiteSpace(item, task, includeForeignId: true);

                if (!ModelState.IsValidUpdated())
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await AddToDefaultTaskIfNotExists(task.Id, item, task);

                await DbContext.SaveChangesAsync();
            }

            item.Id = task.Id;
            item.Name = task.Name;
            return Json(item);
        }

        // PUT api/values/5
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patch(long id, [FromBody]TimeTaskApi item)
        {
            var task = await DbContext.Tasks.Where(t => t.Group.PartitionId == CurrentPartitionId && t.Id == id).FirstOrDefaultAsync();
            if (task == null)
            {
                throw new NotFoundResultException($"Task (TaskId:{id}) not found.");
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, task, includeForeignId: true);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            await AddToDefaultTaskIfNotExists(id, item, task);

            await DbContext.SaveChangesAsync();

            item.Id = id;
            return Json(item);
        }

        private async Task AddToDefaultTaskIfNotExists(long id, TimeTaskApi item, Ttask task)
        {
            if (item.Date.HasValue && item.Date.Value.Date == DateTime.Now.Date)
            {
                if (await DbContext.DefaultTasks.Where(td => td.UserId == CurrentUserId && td.TaskId == id).CountAsync() <= 0)
                {
                    DbContext.DefaultTasks.Add(new DefaultTask { UserId = CurrentUserId, Task = task });
                }
            }
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, [FromBody]TimeTaskApi item) 
        {
            if (!item.Date.HasValue)
            {
                ModelState.AddModelError("Date", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }

            var task = await DbContext.Tasks.Where(t => t.Group.PartitionId == CurrentPartitionId && t.Id == id).FirstOrDefaultAsync();
            if (task == null)
            {
                throw new NotFoundResultException($"Task (TaskId:{id}) not found.");
            }

            var taskItem = await DbContext.TaskItems.Where(w => w.UserId == CurrentUserId && w.TaskId == id && w.Date == item.Date).FirstOrDefaultAsync();
            if (taskItem != null)
            {
                DbContext.TaskItems.Remove(taskItem);
            }

            var defaultTaskTask = await DbContext.DefaultTasks.Where(tdt => tdt.UserId == CurrentUserId && tdt.TaskId == id).FirstOrDefaultAsync();
            if (defaultTaskTask != null)
            {
                DbContext.DefaultTasks.Remove(defaultTaskTask);
            }

            await DbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
