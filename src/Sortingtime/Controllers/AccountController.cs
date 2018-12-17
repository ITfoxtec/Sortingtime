using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sortingtime.Models;
using Sortingtime.ViewModels;
using System.Security.Claims;
using Sortingtime.Infrastructure;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Sortingtime.Infrastructure.Translation;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Linq;
using Sortingtime.Infrastructure.Localization;
using Sortingtime.Controllers.Logic;

namespace Sortingtime.Controllers
{
    public class AccountController : PageController
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly EmailMessageProvider emailMessageProvider;

        public AccountController(TelemetryClient telemetryClient, ILogger<AccountController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, EmailMessageProvider emailMessageProvider) 
        {
            this.telemetryClient = telemetryClient;
            this.logger = logger;
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailMessageProvider = emailMessageProvider;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.Translate = new Translate(); 
            ViewBag.HideMenuLogin = true;
            return View(new LoginViewModel { RememberMe = true });
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var translate = new Translate();
            ViewBag.Translate = translate;
            ViewBag.HideMenuLogin = true;

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    await SendConfirmationEmail(translate, model.Email, user);
                    ModelState.AddModelError("", "We have sent a confirmation to your email address. Please confirm your email.");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    telemetryClient.TrackEvent("LoginSuccess", new Dictionary<string, string> { { "Username" , model.Email } });
                    return RedirectToAction("Index", "A");
                }
                else if (result.IsLockedOut)
                {
                    telemetryClient.TrackEvent("LoginLockedOutFailure", new Dictionary<string, string> { { "Username", model.Email } });
                    ModelState.AddModelError("", "User is locked out, try again later.");
                    return View(model);
                }
                else
                {
                    telemetryClient.TrackEvent("LoginFailure", new Dictionary<string, string> { { "Username", model.Email } });
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            telemetryClient.TrackEvent("Logoff");
            ViewBag.Translate = new Translate();
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "E", Request.RouteCulture());
        }

        //
        // GET: /Account/Register        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;            
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var translate = new Translate();
            ViewBag.Translate = translate;
            ViewBag.HideMenuLogin = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { FullName = model.FullName, UserName = model.Email, Email = model.Email };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        telemetryClient.TrackEvent("RegisterSuccess", new Dictionary<string, string> { { "Username", model.Email } });

                        var addClaimResult = await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Partition, Convert.ToString(CreateNewPartitionAndOrganizationForUser(user, model.Plan))));
                        if (addClaimResult.Succeeded)
                        {
                            await SendConfirmationEmail(translate, model.Email, user);

                            telemetryClient.TrackEvent("RegisterConfirmationEmailSendSuccess", new Dictionary<string, string> { { "Username", model.Email } });

                            await signInManager.SignInAsync(user, isPersistent: true);
                            return RedirectToAction("Index", "A");
                        }
                        else
                        {
                            AddErrors(addClaimResult);
                        }
                    }
                    else
                    {
                        telemetryClient.TrackEvent("RegisterFailure", new Dictionary<string, string> { { "Username", model.Email } });
                        AddErrors(result);
                    }
                }
                catch
                {
                    telemetryClient.TrackEvent("RegisterFatalError", new Dictionary<string, string> { { "Username", model.Email } });
                    throw;
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/Demo
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demo()
        {
            var translate = new Translate();

            var demoData = new DemoData(dbContext, translate);
            var uniqDemoId = await demoData.CreateDemo(Request.HttpContext.Connection.RemoteIpAddress.ToString());

            var user1FullName = translate.Get("DEMO.YOUR_FULL_NAME");
            var user1Username = $"{uniqDemoId}1demo@sortingtime.com";
            var user1 = new ApplicationUser { FullName = user1FullName, UserName = user1Username, Email = user1Username };
            ThrowErrors(await userManager.CreateAsync(user1, Guid.NewGuid().ToString() + "Ab@1"));
            var partitionId = CreateNewPartitionAndOrganizationForUser(user1, Plans.Demo, translate.Get("DEMO.ORGANIZATION_NAME"), translate.Get("DEMO.ORGANIZATION_ADDRESS"));
            ThrowErrors(await userManager.AddClaimAsync(user1, new Claim(CustomClaimTypes.Partition, Convert.ToString(partitionId))));
            ThrowErrors(await userManager.AddClaimAsync(user1, new Claim(CustomClaimTypes.Demo, "true")));

            var user2FullName = translate.Get("DEMO.ANOTHER_USERS_FULL_NAME");
            var user2Username = $"{uniqDemoId}2demo@sortingtime.com";
            var user2 = new ApplicationUser { FullName = user2FullName, UserName = user2Username, Email = user2Username };
            ThrowErrors(await userManager.CreateAsync(user2, Guid.NewGuid().ToString() + "Ab@1"));
            ThrowErrors(await userManager.AddClaimAsync(user2, new Claim(CustomClaimTypes.Partition, Convert.ToString(partitionId))));
            ThrowErrors(await userManager.AddClaimAsync(user2, new Claim(CustomClaimTypes.Demo, "true")));

            telemetryClient.TrackEvent("DemoUsersSuccess", new Dictionary<string, string> { { "partitionId", Convert.ToString(partitionId) }, { "user1Username", user1Username }, { "user2Username", user2Username } });

            demoData.AddDemoData(partitionId, user1, user2);
            await dbContext.SaveChangesAsync();

            telemetryClient.TrackEvent("DemoSuccess", new Dictionary<string, string> { { "partitionId", Convert.ToString(partitionId) } });

            await signInManager.SignInAsync(user1, isPersistent: false);
            return RedirectToAction("Index", "A");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || IsDemoUser(user))
            {
                logger.LogError("Confirm email failed user do not exist or is demo user. User " + user.Email);
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                telemetryClient.TrackEvent("ConfirmEmailSuccess", new Dictionary<string, string> { { "Username", user.Email } });
            }
            else
            {
                telemetryClient.TrackEvent("ConfirmEmailFailure", new Dictionary<string, string> { { "Username", user.Email } });
                logger.LogError("Confirm email failed to complete. User " + user.Email);
            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var translate = new Translate();
            ViewBag.Translate = translate;
            ViewBag.HideMenuLogin = true;

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null || IsDemoUser(user))
                {
                    logger.LogError("Forgot password user do not exist or is demo user. User " + user.Email);
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                await ForgotPasswordEmail(translate, model, user);

                return RedirectToAction("ForgotPasswordConfirmation", Request.RouteCulture());
            }

            ModelState.AddModelError("", string.Format("We could not locate an account with email : {0}", model.Email));

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            return code == null ? View("Error") : View(new ResetPasswordViewModel() { Code = code });
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null || IsDemoUser(user))
            {
                logger.LogError("Reset password user do not exist or is demo user. User " + user.Email);
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account", Request.RouteCulture());
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                telemetryClient.TrackEvent("ResetPasswordSuccess", new Dictionary<string, string> { { "Username", user.Email } });
                await AutoConfirmEmail(user);
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "A");
            }
            else
            {
                telemetryClient.TrackEvent("ResetPasswordFailure", new Dictionary<string, string> { { "Username", user.Email } });
            }

            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            return View();
        }

        [AllowAnonymous]
        public ActionResult FirstLogin(string code)
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            return code == null ? View("Error") : View(new FirstLoginViewModel() { Code = code });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FirstLogin(FirstLoginViewModel model)
        {
            ViewBag.Translate = new Translate();
            ViewBag.HideMenuLogin = true;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null || IsDemoUser(user))
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("", "The email do not match the registered email address or you have been logged in for the first time.");
                return View();
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                await AutoConfirmEmail(user);

                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "A");
            }

            foreach (var error in result.Errors)
            {
                if(error.Code.Equals("InvalidToken", StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("", "The email do not match the registered email address or you have been logged in for the first time.");
                }
                else
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        #region Helpers
        private async Task AutoConfirmEmail(ApplicationUser user)
        {
            if (!user.EmailConfirmed)
            {
                telemetryClient.TrackEvent("AutoConfirmEmail", new Dictionary<string, string> { { "Username", user.Email } });

                string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    telemetryClient.TrackEvent("AutoConfirmEmailSuccess", new Dictionary<string, string> { { "Username", user.Email } });
                }
                else
                {
                    telemetryClient.TrackEvent("AutoConfirmEmailFailed", new Dictionary<string, string> { { "Username", user.Email } });
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("AutoConfirmEmail failed for user " + user.Email + ", error: " + error.Description);
                    }
                }
            }
        }

        private async Task SendConfirmationEmail(Translate translate, string email, ApplicationUser user)
        {
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", RouteUserIdCodeCulture(user.Id, code), protocol: HttpContext.Request.Scheme);

            await emailMessageProvider.SendEmailAsync(
                new MailAddress[] { new MailAddress(email) },
                translate.Get("AUTH.CONFIRMING_EMAIL_EMAIL_HEAD"),
                string.Format(translate.Get("AUTH.CONFIRMING_EMAIL_EMAIL_BODY"), callbackUrl));           
        }

        private async Task ForgotPasswordEmail(Translate translate, ForgotPasswordViewModel model, ApplicationUser user)
        {
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", RouteCodeCulture(code), protocol: HttpContext.Request.Scheme);

            await emailMessageProvider.SendEmailAsync(
                new MailAddress[] { new MailAddress(model.Email) },
                translate.Get("AUTH.RESET_PASSWORD_EMAIL_HEAD"),
                string.Format(translate.Get("AUTH.RESET_PASSWORD_EMAIL_BODY"), callbackUrl));            
        }

        private object RouteUserIdCodeCulture(long userId, string code)
        {
            var culture = Request.Query["culture"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(culture))
            {
                return new { userId = userId, code = code };
            }

            return new { culture = culture, userId = userId, code = code };
        }

        private object RouteCodeCulture(string code)
        {
            var culture = Request.Query["culture"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(culture))
            {
                return new { code = code };
            }

            return new { culture = culture, code = code };
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private void ThrowErrors(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description).ToArray()));
            }
        }

        private long CreateNewPartitionAndOrganizationForUser(ApplicationUser user, Plans plan, string demoOrganizationName = null, string demoOrganizationAddress = null)
        {
            var partition = Partition.CreateNew(plan);
            partition.CreatedByUserId = user.Id;
            dbContext.Partitions.Add(partition);
            var organization = new Organization { Partition = partition, Culture = SortingtimeCultures.CultureToFullCultureName() };
            if(organization.Culture.Equals("da-DK", StringComparison.OrdinalIgnoreCase))
            {
                organization.VatPercentage = 25;
            }
            if(plan == Plans.Demo)
            {
                organization.Name = demoOrganizationName;
                organization.Address = demoOrganizationAddress;
                organization.FirstInvoiceNumber = 1040;
            }
            dbContext.Organizations.Add(organization);
            dbContext.SaveChanges();
            return partition.Id;
        }

        private bool IsDemoUser(ApplicationUser user)
        {
            if (dbContext.UserClaims.Where(c => c.UserId == user.Id && c.ClaimType == CustomClaimTypes.Demo).Count() > 0)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}