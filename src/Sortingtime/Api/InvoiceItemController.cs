using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class InvoiceItemController : SecureApiController
    {
        public InvoiceItemController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                if (!fromDate.HasValue)
                    ModelState.AddModelError("fromDate", "Required Date is null or empty.");
                if (!toDate.HasValue)
                    ModelState.AddModelError("toDate", "Required Date is null or empty.");
                return new BadRequestObjectResult(ModelState);
            }

            var fromDateValue = fromDate.Value;
            var toDateValue = toDate.Value;

            var invoices = await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.Status != InvoiceStatus.Deleted && i.InvoiceDate >= fromDateValue && i.InvoiceDate <= toDateValue)
                .Select(i => new InvoiceItemApi
                {
                    Id = i.Id,
                    Status = i.Status,
                    Number = i.Number,
                    CustomerShort = i.CustomerShort,
                    SubTotalPrice = i.SubTotalPrice,
                    ToEmail = i.ToEmail,
                }).ToListAsync();

            return Json(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var invoice = await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.Status != InvoiceStatus.Deleted && i.Id == id)
                .Select(i => new InvoiceItemApi
                {
                    Id = i.Id,
                    Status = i.Status,
                    Number = i.Number,
                    CustomerShort = i.CustomerShort,
                    InvoiceDate = i.InvoiceDate,
                    SubTotalPrice = i.SubTotalPrice,
                    ToEmail = i.ToEmail,
                    EmailSubject = i.EmailSubject,
                    EmailBody = i.EmailBody,
                }).FirstOrDefaultAsync();

            if (invoice == null)
            {
                return new NotFoundResult();
            }
            return Json(invoice);
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var invoice = await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.Id == id).FirstOrDefaultAsync();
            if (invoice == null)
            {
                return new NotFoundResult();
            }

            invoice.Status = InvoiceStatus.Deleted;
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
