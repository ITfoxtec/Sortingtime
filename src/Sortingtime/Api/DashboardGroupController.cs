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
    public class DashboardGroupController : SecureApiController
    {
        private readonly ILogger logger;
        public DashboardGroupController(ILogger<InvoicingController> logger, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.logger = logger;
        }        
      
        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]TimeGroupApi item)
        {
            await CleanWhiteSpace(item);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var group = await DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId && g.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
            if (group == null)
            {
                group = new Group { PartitionId = CurrentPartitionId, Name = item.Name };
                DbContext.Groups.Add(group);

                if (item.Date.HasValue && item.Date.Value.Date == DateTime.Now.Date)
                {
                    DbContext.DefaultGroups.Add(new DefaultGroup { UserId = CurrentUserId, Group = group });
                }

                await DbContext.SaveChangesAsync();
            }
            else
            {
                await MapDeltaValuesAndCleanWhiteSpace(item, group);

                if (!ModelState.IsValidUpdated())
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await AddToDefaultGroupIfNotExists(group.Id, item, group);

                await DbContext.SaveChangesAsync();
            }

            item.Id = group.Id;
            item.Name = group.Name;
            return Json(item);
        }

        // PUT api/values/5
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patch(long id, [FromBody]TimeGroupApi item)
        {
            var group = await DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId && g.Id == id).FirstOrDefaultAsync();
            if (group == null)
            {
                throw new NotFoundResultException($"Group (GroupId:{id}) not found.");
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, group);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            await AddToDefaultGroupIfNotExists(id, item, group);

            await DbContext.SaveChangesAsync();

            item.Id = id;
            return Json(item);
        }

        private async Task AddToDefaultGroupIfNotExists(long id, TimeGroupApi item, Group group)
        {
            if (item.Date.HasValue && item.Date.Value.Date == DateTime.Now.Date)
            {
                if (!(await DbContext.DefaultGroups.Where(td => td.UserId == CurrentUserId && td.GroupId == id).AnyAsync()))
                {
                    DbContext.DefaultGroups.Add(new DefaultGroup { UserId = CurrentUserId, Group = group });
                }
            }
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, [FromBody]TimeGroupApi item)
        {
            if (!item.Date.HasValue)
            {
                ModelState.AddModelError("Date", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }

            var group = await DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId && g.Id == id).FirstOrDefaultAsync();
            if (group == null)
            {
                throw new NotFoundResultException($"Group (GroupId:{id}) not found.");
            }

            await RemoveSubElements(id, item.Date.Value);

            var defaultTaskGroup = await DbContext.DefaultGroups.Where(tdg => tdg.UserId == CurrentUserId && tdg.GroupId == id).FirstOrDefaultAsync();
            if (defaultTaskGroup != null)
            {
                DbContext.DefaultGroups.Remove(defaultTaskGroup);
            }

            await DbContext.SaveChangesAsync();
            return Ok();
        }

        private async Task RemoveSubElements(int id, DateTime date)
        {
            foreach (var task in DbContext.Tasks.Where(t => t.GroupId == id))
            {
                var taskItem = await DbContext.TaskItems.Where(w => w.UserId == CurrentUserId && w.Task == task && w.Date == date).FirstOrDefaultAsync();
                if (taskItem != null)
                {
                    DbContext.TaskItems.Remove(taskItem);
                }

                var defaultTaskTime = await DbContext.DefaultTasks.Where(tdt => tdt.UserId == CurrentUserId && tdt.Task == task).FirstOrDefaultAsync();
                if (defaultTaskTime != null)
                {
                    DbContext.DefaultTasks.Remove(defaultTaskTime);
                }
            }

            foreach (var material in DbContext.Materials.Where(t => t.GroupId == id))
            {
                var materialItem = await DbContext.MaterialItems.Where(w => w.UserId == CurrentUserId && w.Material == material && w.Date == date).FirstOrDefaultAsync();
                if (materialItem != null)
                {
                    DbContext.MaterialItems.Remove(materialItem);
                }

                var defaultMaterial = await DbContext.DefaultMaterials.Where(tdt => tdt.UserId == CurrentUserId && tdt.Material == material).FirstOrDefaultAsync();
                if (defaultMaterial != null)
                {
                    DbContext.DefaultMaterials.Remove(defaultMaterial);
                }
            }
        }
    }
}
