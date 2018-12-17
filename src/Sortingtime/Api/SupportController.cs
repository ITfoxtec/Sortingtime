using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sortingtime.ApiModels;
using Sortingtime.Infrastructure;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Api
{
    [Route("api/[controller]")]
    public class SupportController : SecureApiController
    {
        private readonly EmailMessageProvider emailMessageProvider;

        public SupportController(EmailMessageProvider emailMessageProvider, ApplicationDbContext dbContext) : base(dbContext)
        {
            this.emailMessageProvider = emailMessageProvider;
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]SupportApi item)
        {
            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var translate = new Translate();

            var user = await DbContext.Users.Where(u => u.Id == CurrentUserId).Select(u => new { u.FullName, u.Email }).FirstAsync();

            await emailMessageProvider.SendEmailAsync(
                    new MailAddress[] { new MailAddress("support@sortingtime.com") },
                    $"{translate.Get("SUPPORT.SUPPORT")} [id:{DateTime.Now.Ticks}]", item.Message.ToHtml(),
                    fromEmail: new MailAddress(user.Email, user.FullName));

            return Ok();
        }
    }
}
