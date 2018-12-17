using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Models;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Sortingtime.Api.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : SecureApiController
    {
        private readonly ILogger logger;
        public DashboardController(ILogger<InvoicingController> logger, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.logger = logger;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(DateTime? date, string[] cuid)
        {
            if (!date.HasValue)
            {
                ModelState.AddModelError("date", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }
            // Only pass Date through
            date = date.Value.Date;

            var firstDayOfMonth = date.Value.FirstDayOfMonth();
            var firstDayOfNextMonth = date.Value.FirstDayOfNextMonth();
            var firstDayOfWeek = date.Value.FirstDayOfWeek();
            var firstDayOfNextWeek = date.Value.FirstDayOfNextWeek();

            var timeGroups = DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId &&
                (DbContext.DefaultGroups.Any(tdg => tdg.UserId == CurrentUserId && tdg.GroupId == g.Id) ||
                g.Tasks.Where(t => t.Items.Where(i => i.UserId == CurrentUserId && i.Date == date.Value).Count() > 0 || DbContext.DefaultTasks.Any(dt => dt.UserId == CurrentUserId && dt.Task == t)).Count() > 0 || // GroupsWhereTasks, work around code copied in
                g.Materials.Where(m => m.Items.Where(i => i.UserId == CurrentUserId && i.Date == date.Value).Count() > 0 || DbContext.DefaultMaterials.Any(dm => dm.UserId == CurrentUserId && dm.Material == m)).Count() > 0)) // GroupsWhereMaterials, work around code copied in
                .Select(g => new TimeGroupApi
                {
                    Id = g.Id,
                    Name = g.Name,
                    Tasks = g.Tasks.Where(t => t.Items.Where(i => i.UserId == CurrentUserId && i.Date == date.Value).Count() > 0 || DbContext.DefaultTasks.Any(dt => dt.UserId == CurrentUserId && dt.Task == t))  // GroupsWhereTasks, work around code copied in
                        .Select(t => new TimeTaskApi
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Item = t.Items.Where(i => i.UserId == CurrentUserId && i.Date == date.Value).Select(w => new TimeTaskItemApi
                            {
                                Id = w.Id,
                                Time = w.Time,
                            }).FirstOrDefault(),
                        }).OrderBy(t => t.Name).ToList(),
                    DayTaskTotal = DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Task.GroupId == g.Id && ti.Date == date.Value).Select(ti => (int)ti.Time).Sum(),
                    WeekTaskTotal = DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Task.GroupId == g.Id && ti.Date >= firstDayOfWeek && ti.Date < firstDayOfNextWeek).Select(ti => (int)ti.Time).Sum(),
                    MonthTaskTotal = DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.Task.GroupId == g.Id && ti.Date >= firstDayOfMonth && ti.Date < firstDayOfNextMonth).Select(ti => (int)ti.Time).Sum(),
                }).OrderBy(g => g.Name).ToList(); // do not currently work with Async...


            var uniqIdCount = 1;
            var pageItems = new List<DashboardItemApi>();
            foreach (var group in timeGroups)
            {
                pageItems.Add(new DashboardItemApi
                {
                    Type = DashboardItemTypes.Group,
                    UniqId = GroupUniquId(date.Value, cuid, group),
                    Id = group.Id,
                    Name = group.Name,
                    DayTaskTotal = group.DayTaskTotal,
                    WeekTaskTotal = group.WeekTaskTotal,
                    MonthTaskTotal = group.MonthTaskTotal
                });

                foreach (var task in group.Tasks)
                {
                    pageItems.Add(new DashboardItemApi
                    {
                        Type = DashboardItemTypes.Task,
                        UniqId = GroupTaskUniquId(date.Value, cuid, group, task),
                        Id = task.Id,
                        GroupId = group.Id,
                        Name = task.Name,
                        TaskItemId = task.Item?.Id,
                        Time = task.Item?.Time,
                        WeekTaskTotal = await EntityFrameworkQueryableExtensions.SumAsync(DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.TaskId == task.Id && ti.Date >= firstDayOfWeek && ti.Date < firstDayOfNextWeek).Select(ti => (int)ti.Time)),
                        MonthTaskTotal = await EntityFrameworkQueryableExtensions.SumAsync(DbContext.TaskItems.Where(ti => ti.UserId == CurrentUserId && ti.TaskId == task.Id && ti.Date >= firstDayOfMonth && ti.Date < firstDayOfNextMonth).Select(ti => (int)ti.Time))
                    });
                }

                pageItems.Add(new DashboardItemApi { State = DashboardItemStates.Button, Type = DashboardItemTypes.Task, GroupId = group.Id, UniqId = SpaceUniquId(date.Value, uniqIdCount++) });  
            }
            pageItems.Add(new DashboardItemApi { State = DashboardItemStates.Button,  Type = DashboardItemTypes.Group, UniqId = SpaceUniquId(date.Value, uniqIdCount++) });  



            return Json(pageItems);
        }

        private string GroupUniquId(DateTime date, string[] changedUniqIds, TimeGroupApi group)
        {
            var uniquId = $"g{group.Id}";
            return ChangedUniquId(date, changedUniqIds, uniquId);
        }

        private string GroupTaskUniquId(DateTime date, string[] changedUniqIds, TimeGroupApi group, TimeTaskApi task)
        {
            var uniquId = $"g{group.Id}t{task.Id}{(task.Item?.Id != null ? $"ti{task.Item?.Id}" : "")}";
            return ChangedUniquId(date, changedUniqIds, uniquId);
        }

        private string ChangedUniquId(DateTime date, string[] changedUniqIds, string uniquId)
        {
            if (changedUniqIds != null && changedUniqIds.Contains(uniquId))
            {
                return uniquId + $"y{date.Year}m{date.Month}d{date.Day}";
            }
            else
            {
                return uniquId;
            }
        }

        private string SpaceUniquId(DateTime date, int uniqId)
        {
            return $"sy{date.Year}m{date.Month}s{date.Day}c{uniqId}";
        }
    }
}
