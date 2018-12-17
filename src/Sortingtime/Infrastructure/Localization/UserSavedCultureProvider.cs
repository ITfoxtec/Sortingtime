using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Sortingtime.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace Sortingtime.Infrastructure
{
    public class UserSavedCultureProvider : IRequestCultureProvider
    {
        public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                long currentPartitionId = httpContext.User.GetPartitionId();

                using (var taskServiceScope = httpContext.RequestServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var dbContext = taskServiceScope.ServiceProvider.GetService<ApplicationDbContext>();

                    string cultureName = await dbContext.Organizations.Where(o => o.Id == currentPartitionId).Select(u => u.Culture).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(cultureName))
                    {
                        return await Task.FromResult(new ProviderCultureResult(cultureName));
                    }
                }
            }

            return await Task.FromResult<ProviderCultureResult>(null);
        }
    }
}
