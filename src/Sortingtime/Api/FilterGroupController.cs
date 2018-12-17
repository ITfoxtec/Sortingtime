using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class FilterGroupController : SecureApiController
    {
        public FilterGroupController(ApplicationDbContext dbContext) : base(dbContext)
		{ }

        // next time, create JS call with exclude ids

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string filter, [FromQuery(Name = "eId")] IEnumerable<long> excludeIds)
        {
            var groupQuery = DbContext.Groups.Where(g => g.PartitionId == CurrentPartitionId);

            if(excludeIds?.Count() > 0)
            {
                groupQuery = groupQuery.Where(g => !excludeIds.Contains(g.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                groupQuery = groupQuery.Where(g => EF.Functions.Like(g.Name, $"%{filter}%"));
            }

            var groups = await groupQuery.OrderBy(g => g.Name)
                .Take(10).Select(g => new FilterItemApi { Id = g.Id, Value = g.Name }).ToListAsync();
            return Json(groups);
        }
    }
}
