using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class LogoController : SecureApiController
    {
        public LogoController(ApplicationDbContext dbContext) : base(dbContext)
		{ }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var imageUrl = await DbContext.Organizations.Where(o => o.Id == CurrentPartitionId).Select(o => o.Logo).FirstOrDefaultAsync();

            return Json(new LogoApi { Image = imageUrl });
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]LogoApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var imageUrl = ConvertImageUrl(item.Image);
            if(imageUrl == null)
            {
                ModelState.AddModelError("item.Image", "Invalid image.");
                return new BadRequestObjectResult(ModelState);
            }

            var organization = await DbContext.Organizations.Where(o => o.Id == CurrentPartitionId).SingleOrDefaultAsync();
            if (organization == null)
            {
                organization = new Organization
                {
                    Id = CurrentPartitionId
                };
                DbContext.Organizations.Add(organization);
            }

            organization.Logo = imageUrl;

            await DbContext.SaveChangesAsync();

            return Json(new LogoApi { Image = imageUrl });
        }

        private string ConvertImageUrl(string imageUrl)
        {
            var imageSplit = imageUrl.Split(',');
            if (imageSplit.Length <= 1)
            {
                return null;
            }

            using (var ms = new MemoryStream(Convert.FromBase64String(imageSplit[1])))
            using (var image = new Bitmap(ms))
            {
                image.MakeTransparent();

                using (var resizedImage = image.ResizeImage(new Size(160, 120)))
                {
                    return CreateImageUrl("png", Convert.ToBase64String(resizedImage.ToByteArray(ImageFormat.Png)));
                }
            }       
        }

        private string CreateImageUrl(string type, string imageBase64)
        {
            return String.Format(@"data:image/{0};base64,{1}", type, imageBase64);
        }

    }
}
