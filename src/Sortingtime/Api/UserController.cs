using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Infrastructure;
using Sortingtime.ApiModels;
using Sortingtime.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Sortingtime.Infrastructure.Translation;
using Microsoft.AspNetCore.Identity;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sortingtime.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : SecureApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmailMessageProvider emailMessageProvider;

        public UserController(UserManager<ApplicationUser> userManager, EmailMessageProvider emailMessageProvider, ApplicationDbContext dbContext) : base(dbContext)
		{
            this.userManager = userManager;
            this.emailMessageProvider = emailMessageProvider;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {            
            var users = await DbContext.Users.Where(u => 
                    u.Claims.Count(c => c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionIdAsString) > 0
                ).Select(u => new UserApi { Id = u.Id, FullName = u.FullName, Email = u.UserName }).ToListAsync();
            return Json(users);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            if (id == -1)
            {
                id = CurrentUserId;
            }

            var user = await DbContext.Users.Where(u => 
                    u.Claims.Count(c => c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionIdAsString) > 0 && 
                    u.Id == id
                ).Select(u => new UserApi { Id = u.Id, FullName = u.FullName, Email = u.UserName }).FirstOrDefaultAsync();
            if (user == null)
            {
                return new NotFoundResult();
            }
            return Json(user);
        }

        // POST api/values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]UserApi item)
        {
            await CleanWhiteSpace(item);

            if (!ModelState.IsValidUpdated())
            {
                return new BadRequestObjectResult(ModelState);
            }

            var translate = new Translate();

            var partition = await DbContext.Partitions.Where(p => p.Id == CurrentPartitionId).Select(p => new { IsDemo = p.Plan == Plans.Demo, p.MaxUsers }).FirstAsync();
            //Not in demo mode
            if (partition.IsDemo)
            {
                ModelState.AddModelError("", translate.Get("AUTH.FUNCTIONALITY_NOT_AVAILABLE_ERROR"));
                return new BadRequestObjectResult(ModelState);
            }
            // Max user count
            if (await DbContext.UserClaims.Where(c => c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionIdAsString).CountAsync() >= partition.MaxUsers)
            {
                ModelState.AddModelError("", translate.Get("AUTH.EXCEEDS_MAXIMUM_USERS_ERROR"));
                return new BadRequestObjectResult(ModelState);
            }

            var user = new ApplicationUser { FullName = item.FullName, UserName = item.Email, Email = item.Email };
            var result = await userManager.CreateAsync(user, Guid.NewGuid().ToString() + "Ab@1");
            if (result.Succeeded)
            {
                var addClaimResult = await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Partition, CurrentPartitionIdAsString));
                if (addClaimResult.Succeeded)
                {
                    await SetPasswordEmail(item.Email, user);
                }
                else
                {
                    foreach (var error in addClaimResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return new BadRequestObjectResult(ModelState);
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return new BadRequestObjectResult(ModelState);
            }

            return Json(new UserApi { Id = user.Id, FullName = user.FullName, Email = user.Email });
        }

        // PUT api/values/5
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patch(long id, [FromBody]UserApi item)
        {
            ModelState.ClearValidationState("");

            var user = await DbContext.Users.Where(u =>
                    u.Claims.Count(c => c.ClaimType == CustomClaimTypes.Partition && c.ClaimValue == CurrentPartitionIdAsString) > 0 &&
                    u.Id == id
                ).FirstOrDefaultAsync();
            if (user == null)
            {
                return new NotFoundResult();
            }

            if (item.Password != null)
            {
                var translate = new Translate();

                //Not in demo mode
                if (await DbContext.Partitions.Where(p => p.Id == CurrentPartitionId).Select(p => p.Plan == Plans.Demo).FirstAsync())
                {
                    ModelState.AddModelError("", translate.Get("AUTH.FUNCTIONALITY_NOT_AVAILABLE_ERROR"));
                    return new BadRequestObjectResult(ModelState);
                }

                string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(user, resetToken, item.Password);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var passError in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("item.Password", passError.Description);
                    }
                    return new BadRequestObjectResult(ModelState);
                }
            }
            else
            {
                await MapDeltaValuesAndCleanWhiteSpace(item, user);

                if (item.Email != null)
                {
                    user.UserName = item.Email;
                }

                if (!ModelState.IsValidUpdated())
                {
                    return new BadRequestObjectResult(ModelState);
                }

                DbContext.SaveChanges();
            }

            return Json(null);
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public ActionResult Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public ActionResult Delete(int id)
        //{
        //}

        //private async Task SendConfirmationEmail(UserModel model, ApplicationUser user)
        //{
        //    string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

        //    await MessageServices.SendEmailAsync(model.Email, "Confirm your account", "Please confirm your email by clicking this link: <a href=\"" + callbackUrl + "\">link</a>", cancellationToken: HttpContext.RequestAborted);
        //}

        private async Task SetPasswordEmail(string email, ApplicationUser user)
        {
            var translate = new Translate();

            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("FirstLogin", "Account", new { code = code }, protocol: HttpContext.Request.Scheme);
            await emailMessageProvider.SendEmailAsync(
                new MailAddress[] { new MailAddress(email) },
                translate.Get("AUTH.FIRST_LOGIN_EMAIL_HEAD"), 
                string.Format(translate.Get("AUTH.FIRST_LOGIN_EMAIL_BODY"), callbackUrl));
        }

    }
}
