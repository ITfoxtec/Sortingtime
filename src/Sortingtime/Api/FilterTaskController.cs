using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Sortingtime.ApiModels;
using System.Collections.Generic;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class FilterTaskController : SecureApiController
    {
        public FilterTaskController(ApplicationDbContext dbContext) : base(dbContext)
		{ }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string filter, long groupId, [FromQuery(Name = "eId")] IEnumerable<long> excludeIds)
        {
            var taskQuery = DbContext.Tasks.Where(t => t.Group.PartitionId == CurrentPartitionId && t.Group.Id == groupId);

            if (excludeIds?.Count() > 0)
            {
                taskQuery = taskQuery.Where(g => !excludeIds.Contains(g.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                taskQuery = taskQuery.Where(t => EF.Functions.Like(t.Name, $"%{filter}%"));
            }

            var tasks = await taskQuery.OrderBy(t => t.Name)
                .Take(10).Select(t => new FilterItemApi { Id = t.Id, Value = t.Name }).ToListAsync();
            return Json(tasks);
        }

    }
}
