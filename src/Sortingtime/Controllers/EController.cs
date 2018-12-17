using Sortingtime.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sortingtime.Models;
using System.Linq;
using System;
using Sortingtime.Infrastructure.Translation;
using Sortingtime.ViewModels;
using Sortingtime.Infrastructure.Localization;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sortingtime.Controllers
{
    public class EController : PageController
    {
        private readonly ILogger logger;
        private readonly ApplicationDbContext dbContext;
        private readonly EmailMessageProvider emailMessageProvider;

        public EController(ILogger<EController> logger, ApplicationDbContext dbContext, EmailMessageProvider emailMessageProvider) : base()
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.emailMessageProvider = emailMessageProvider;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "A");
            }

            ViewBag.Translate = new Translate();
            return View();
        }

        public IActionResult LicensingTerms()
        {
            ViewBag.Translate = new Translate();
            return View();
        }

        public IActionResult Terms()
        {
            ViewBag.Translate = new Translate();
            return View();
        }

        public IActionResult Support()
        {
            ViewBag.Translate = new Translate();
            return View();
        }

        //
        // POST: /E/Support
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Support(SupportViewModel model)
        {
            var translate = new Translate();
            ViewBag.Translate = translate;

            if (ModelState.IsValid)
            {
                await emailMessageProvider.SendEmailAsync(
                   new MailAddress[] { new MailAddress("support@sortingtime.com") },
                   $"{translate.Get("SUPPORT.SUPPORT")} [id:{DateTime.Now.Ticks}]", model.Message.ToHtml(),
                   fromEmail: new MailAddress(model.Email, model.FullName));

                return RedirectToAction("SupportConfirmation", Request.RouteCulture());
            }

            return View(model);
        }

        public ActionResult SupportConfirmation()
        {
            ViewBag.Translate = new Translate();
        
            return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {            
            ViewBag.Translate = new Translate();
            return View();
        }

#if DEBUG
        public IActionResult Testgksaiogjhwhpkmfjvepsjf()
        {
            string text = "OK";
            int users = -1;
            int demoPartitions = -1;
            int freePartitions = -1;
            int demoUsers = -1;
            try
            {
                demoPartitions = dbContext.Partitions.Where(p => p.Plan == Plans.Demo).Count();
                freePartitions = dbContext.Partitions.Where(p => p.Plan == Plans.Free).Count();
                users = dbContext.Users.Where(u => u.Claims.Where(c => c.ClaimType == CustomClaimTypes.Demo).Count() <= 0).Count();
                demoUsers = dbContext.Users.Where(u => u.Claims.Where(c => c.ClaimType == CustomClaimTypes.Demo).Count() > 0).Count();
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Error during page test / warm up");
                text = $"Error: {exc.Message}";
            }          

            return new ContentResult
            {
                ContentType = "text/html",
                Content = $"<html><body>{text}<br /><br />FREE Partitions: {freePartitions}<br />Users: {users}<br /><br />Demo Partitions: {demoPartitions}<br />Demo Users: {demoUsers}<br /></body></html>"
            };
        }

        public IActionResult TestgksaiogjhwhpkmfjvepsjfThrow()
        {
            throw new Exception("Throw test error");
        }      
#endif

    }
}
