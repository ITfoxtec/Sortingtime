using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;
using Sortingtime.Infrastructure.Localization;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class OrganizationController : SecureApiController
    {
        public OrganizationController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var organization = await DbContext.Organizations.Where(o => o.Id == CurrentPartitionId).Select(o => new OrganizationApi
            {
                Name = o.Name,
                Address = o.Address,
                VatNumber = o.VatNumber,
                PaymentDetails = o.PaymentDetails,
                TaxPercentage = o.TaxPercentage,
                VatPercentage = o.VatPercentage,
                FirstInvoiceNumber = o.FirstInvoiceNumber,
                Culture = o.Culture
            }).SingleOrDefaultAsync();
            organization.IsDemo = await DbContext.Partitions.Where(p => p.Id == CurrentPartitionId).Select(p => p.Plan == Plans.Demo).FirstAsync();
            organization.UserCount = await DbContext.UserClaims.Where(c => c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionIdAsString).CountAsync();
            return Json(organization);
        }

        // PUT api/values/5
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patch(long id, [FromBody]OrganizationApi item)
        {
            ModelState.ClearValidationState("");

            // id is ignored. Insted CurrentPartitionId is used.

            var organization = await DbContext.Organizations.Where(o => o.Id == CurrentPartitionId).SingleOrDefaultAsync();
            if (organization == null)
            {
                return new NotFoundResult();
            }

            if (!string.IsNullOrEmpty(item.Culture))
            {
                var culture = CultureInfo.GetCultureInfo(item.Culture);
                if (!SortingtimeCultures.IsSupported(culture.Name))
                {
                    ModelState.AddModelError("item.Culture", "Culture not supported.");
                    return new BadRequestObjectResult(ModelState);
                }   
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, organization);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }           

            await DbContext.SaveChangesAsync();

            return Json(item);            
        }

    }
}
