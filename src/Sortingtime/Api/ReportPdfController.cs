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
    public class ReportPdfController : SecureApiController
    {
        public ReportPdfController(ApplicationDbContext dbContext) : base(dbContext)
        { }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var translate = new Translate();

            var report = await DbContext.Reports.Where(r => r.PartitionId == CurrentPartitionId && r.Id == id).Select(r => new { r.Number }).FirstOrDefaultAsync();

            if (report == null)
                return new NotFoundResult();

            var storageAccount = CloudStorageAccount.Parse(Startup.Configuration.GetConnectionString("AzureWebJobsStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var rapportBlobContainer = blobClient.GetContainerReference("report");
            var rapportPdfBlob = rapportBlobContainer.GetBlockBlobReference(id + ".pdf");

            var memoryStream = new MemoryStream();
            await rapportPdfBlob.DownloadToStreamAsync(memoryStream);
            await memoryStream.FlushAsync();
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, "application/pdf")
            {
                FileDownloadName = $"{translate.Get("REPORT.FILENAME")} {report.Number}.pdf",
            };
        }        
    }
}
