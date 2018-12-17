using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sortingtime.Api.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class HourPriceSettingController : SecureApiController
    {
        public HourPriceSettingController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string groupReferenceKey, string taskReferenceKey, long userId)
        {
            if (string.IsNullOrWhiteSpace(taskReferenceKey))
            {
                ModelState.AddModelError("taskReferenceKey", "Invalid Task Reference Key.");
                return new BadRequestObjectResult(ModelState);
            }
            if (userId <= 0 || !(await new UserLogic(DbContext, CurrentPartitionId).IsUserInPartitionId(userId)))
            {
                ModelState.AddModelError("userId", "Invalid User ID Reference Key.");
                return new BadRequestObjectResult(ModelState);
            }

            var hourPriceLogic = new HourPriceLogic(DbContext, CurrentPartitionId);

            return Json(await hourPriceLogic.GetHourPrice(groupReferenceKey, taskReferenceKey, userId));
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]HourPriceSettingApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (!(await new UserLogic(DbContext, CurrentPartitionId).IsUserInPartitionId(item.UserId)))
            {
                ModelState.AddModelError("item.UserId", "Invalid User ID Reference Key.");
                return new BadRequestObjectResult(ModelState);
            }

            var hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId && 
                hs.GroupReferenceKey == item.GroupReferenceKey && hs.TaskReferenceKey == item.TaskReferenceKey && hs.UserId == item.UserId).FirstOrDefaultAsync();

            if(hourPriceSetting == null)
            {
                hourPriceSetting = new HourPriceSetting
                {
                    PartitionId = CurrentPartitionId,
                    GroupReferenceKey = item.GroupReferenceKey,
                    TaskReferenceKey = item.TaskReferenceKey,
                    UserId = item.UserId,
                };
                DbContext.HourPriceSettings.Add(hourPriceSetting);
            }

            hourPriceSetting.Timestamp = DateTime.UtcNow;
            hourPriceSetting.HourPrice = item.HourPrice;

            await DbContext.SaveChangesAsync();

            return Json(item);
        }

    }
}
