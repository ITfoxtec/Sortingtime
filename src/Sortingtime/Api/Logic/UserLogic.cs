using Microsoft.EntityFrameworkCore;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api.Logic
{
    public class UserLogic
    {
        private ApplicationDbContext DbContext { get; }
        private long CurrentPartitionId { get; }

        public UserLogic(ApplicationDbContext dbContext, long currentPartitionId)
        {
            DbContext = dbContext;
            CurrentPartitionId = currentPartitionId;
        }

        public async Task<bool> IsUserInPartitionId(long userId)
        {
            return await DbContext.UserClaims.Where(c => c.UserId == userId && c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionId.ToString()).CountAsync() > 0;
        }
    }
}
