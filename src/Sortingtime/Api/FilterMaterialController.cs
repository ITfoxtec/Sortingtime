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
    public class FilterMaterialController : SecureApiController
    {
        public FilterMaterialController(ApplicationDbContext dbContext) : base(dbContext)
		{ }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string filter, long groupId, [FromQuery(Name = "eId")] IEnumerable<long> excludeIds)
        {
            var materialQuery = DbContext.Materials.Where(m => m.Group.PartitionId == CurrentPartitionId && m.Group.Id == groupId);

            if (excludeIds?.Count() > 0)
            {
                materialQuery = materialQuery.Where(g => !excludeIds.Contains(g.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                materialQuery = materialQuery.Where(m => EF.Functions.Like(m.Name, $"%{filter}%"));
            }

            var tasks = await materialQuery.OrderBy(m => m.Name)
                .Take(10).Select(m => new FilterItemApi { Id = m.Id, Value = m.Name, Price = m.Price }).ToListAsync();
            return Json(tasks);
        }

    }
}
