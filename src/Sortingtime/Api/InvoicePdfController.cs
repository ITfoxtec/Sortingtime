using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class InvoicePdfController : SecureApiController
    {
        public InvoicePdfController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var translate = new Translate();

            var invoice = await DbContext.Invoices.Where(i => i.PartitionId == CurrentPartitionId && i.Id == id).Select(i => new { i.CreditNote, i.Number }).FirstOrDefaultAsync();

            if (invoice == null)
                return new NotFoundResult();

            var storageAccount = CloudStorageAccount.Parse(Startup.Configuration.GetConnectionString("AzureWebJobsStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var invoiceBlobContainer = blobClient.GetContainerReference("invoice");
            var invoicePdfBlob = invoiceBlobContainer.GetBlockBlobReference(id + ".pdf");

            var memoryStream = new MemoryStream();
            await invoicePdfBlob.DownloadToStreamAsync(memoryStream);
            await memoryStream.FlushAsync();
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, "application/pdf")
            {
                FileDownloadName = $"{(!invoice.CreditNote ? translate.Get("INVOICE.FILENAME") : translate.Get("INVOICE.CREDIT_NOTE_FILENAME"))} {invoice.Number}.pdf",
            };
        }
    }
}