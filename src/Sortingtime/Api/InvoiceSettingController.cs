using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class InvoiceSettingController : SecureApiController
    {
        public InvoiceSettingController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string referenceType, string referenceKey)
        {
            var dbReferenceType = 0;
            if (SettingApi.ReferenceTypes.Group.Equals(referenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 10;
            }
            else if (SettingApi.ReferenceTypes.Task.Equals(referenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 20;
            }
            else
            {
                ModelState.AddModelError(referenceType, "Invalid Invoice Settings Reference Type.");
                return new BadRequestObjectResult(ModelState);
            }

            var invoiceSetting = await DbContext.InvoiceSettings.Where(s => s.PartitionId == CurrentPartitionId && s.ReferenceType == dbReferenceType && s.ReferenceKey == referenceKey)
                .Select(s => new InvoiceSettingApi
                {
                    ReferenceType = referenceType,
                    ReferenceKey = referenceKey,
                    ToEmail = s.ToEmail,
                    EmailSubject = s.EmailSubject,
                    EmailBody = s.EmailBody,
                    InvoiceTitle = s.InvoiceTitle,
                    InvoiceCustomer = s.InvoiceCustomer,
                    InvoicePaymentTerms = s.InvoicePaymentTerms,
                    InvoiceReference = s.InvoiceReference,
                    InvoiceText = s.InvoiceText,
                }).FirstOrDefaultAsync();

            if(invoiceSetting == null)
            {
                invoiceSetting = new InvoiceSettingApi
                {
                    ReferenceType = referenceType,
                    ReferenceKey = referenceKey,
                };
            }

            if(string.IsNullOrWhiteSpace(invoiceSetting.InvoiceCustomer) && SettingApi.ReferenceTypes.Group.Equals(referenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                invoiceSetting.InvoiceCustomer = referenceKey;
            }
            return Json(invoiceSetting);
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]InvoiceSettingApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var dbReferenceType = 0;
            if (SettingApi.ReferenceTypes.Group.Equals(item.ReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 10;
            }
            else if (SettingApi.ReferenceTypes.Task.Equals(item.ReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                dbReferenceType = 20;
            }
            else
            {
                ModelState.AddModelError("item.ReferenceType", "Invalid Invoice Settings Reference Type.");
                return new BadRequestObjectResult(ModelState);
            }


            var invoiceSetting = await DbContext.InvoiceSettings.Where(s => s.PartitionId == CurrentPartitionId && s.ReferenceType == dbReferenceType && s.ReferenceKey == item.ReferenceKey).FirstOrDefaultAsync();
            if(invoiceSetting == null)
            {
                invoiceSetting = new InvoiceSetting
                {
                    PartitionId = CurrentPartitionId,
                    ReferenceType = dbReferenceType,
                    ReferenceKey = item.ReferenceKey,
                };
                DbContext.InvoiceSettings.Add(invoiceSetting);
            }

            await MapDeltaValuesAndCleanWhiteSpace(item, invoiceSetting, new string[] { "ReferenceType", "ReferenceKey" }, updateNulls:true);

            await DbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
