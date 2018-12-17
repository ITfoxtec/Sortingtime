using Microsoft.EntityFrameworkCore;
using Sortingtime.ApiModels;
using Sortingtime.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api.Logic
{
    public class HourPriceLogic
    {
        private ApplicationDbContext DbContext { get; }
        private long CurrentPartitionId { get; }

        public HourPriceLogic(ApplicationDbContext dbContext, long currentPartitionId)
        {
            DbContext = dbContext;
            CurrentPartitionId = currentPartitionId;
        }

        public async Task<HourPriceSettingApi> GetHourPrice(string groupReferenceKey, string taskReferenceKey, long userId)
        {
            var hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId &&
                    hs.GroupReferenceKey == groupReferenceKey && hs.TaskReferenceKey == taskReferenceKey && hs.UserId == userId)
                .Select(hs => new HourPriceSettingApi
                {
                    GroupReferenceKey = hs.GroupReferenceKey,
                    TaskReferenceKey = hs.TaskReferenceKey,
                    UserId = hs.UserId,
                    HourPrice = hs.HourPrice
                }).FirstOrDefaultAsync();

            if (hourPriceSetting == null && !string.IsNullOrWhiteSpace(groupReferenceKey))
            {
                hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId &&
                        hs.GroupReferenceKey == groupReferenceKey && hs.UserId == userId)
                    .OrderByDescending(hs => hs.Timestamp)
                    .Select(hs => new HourPriceSettingApi
                    {
                        GroupReferenceKey = hs.GroupReferenceKey,
                        TaskReferenceKey = hs.TaskReferenceKey,
                        UserId = hs.UserId,
                        HourPrice = hs.HourPrice
                    }).FirstOrDefaultAsync();
            }

            if (hourPriceSetting == null)
            {
                hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId &&
                        hs.UserId == userId)
                    .OrderByDescending(hs => hs.Timestamp)
                    .Select(hs => new HourPriceSettingApi
                    {
                        GroupReferenceKey = hs.GroupReferenceKey,
                        TaskReferenceKey = hs.TaskReferenceKey,
                        UserId = hs.UserId,
                        HourPrice = hs.HourPrice
                    }).FirstOrDefaultAsync();
            }

            if (hourPriceSetting == null)
            {
                hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId &&
                        hs.GroupReferenceKey == groupReferenceKey && hs.TaskReferenceKey == taskReferenceKey)
                    .OrderByDescending(hs => hs.Timestamp)
                    .Select(hs => new HourPriceSettingApi
                    {
                        GroupReferenceKey = hs.GroupReferenceKey,
                        TaskReferenceKey = hs.TaskReferenceKey,
                        UserId = hs.UserId,
                        HourPrice = hs.HourPrice
                    }).FirstOrDefaultAsync();
            }

            if (hourPriceSetting == null && !string.IsNullOrWhiteSpace(groupReferenceKey))
            {
                hourPriceSetting = await DbContext.HourPriceSettings.Where(hs => hs.PartitionId == CurrentPartitionId &&
                        hs.GroupReferenceKey == groupReferenceKey)
                    .OrderByDescending(hs => hs.Timestamp)
                    .Select(hs => new HourPriceSettingApi
                    {
                        GroupReferenceKey = hs.GroupReferenceKey,
                        TaskReferenceKey = hs.TaskReferenceKey,
                        UserId = hs.UserId,
                        HourPrice = hs.HourPrice
                    }).FirstOrDefaultAsync();
            }

            return hourPriceSetting;
        }
    }
}
