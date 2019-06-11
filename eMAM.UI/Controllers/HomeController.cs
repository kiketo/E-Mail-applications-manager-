using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.GmailServices.Contracts;
using eMAM.Service.UserServices.Contracts;
using eMAM.UI.Mappers;
using eMAM.UI.Models;
using eMAM.UI.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IGmailApiService gmailApiService;
        private readonly IGmailUserDataService gmailUserDataService;
        private readonly IEmailService emailService;
        private IViewModelMapper<Email, EmailViewModel> emailViewModelMapper;
        private readonly IUserService userService;
        private readonly IAuditLogService auditLogService;
        private readonly IStatusService statusService;
        private readonly ILogger logger;

        public HomeController(
            IGmailApiService gmailApiService,
            IGmailUserDataService gmailUserDataService,
            IEmailService emailService,
            IViewModelMapper<Email, EmailViewModel> emailViewModelMapper,
            IUserService userService,
            IAuditLogService auditLogService,
            IStatusService statusService,
            ILogger<User> logger)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
        }

        //[Authorize]
        public IActionResult Index()
        {


            return View();
        }

        public async Task<IActionResult> ListAllMails(int? pageNumber)
        {
            var mails = this.emailService.ReadAllMailsFromDb();
            var pageSize = 10;
            var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }

            return View(model);
        }

        //[ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PreviewMail(string messageId)
        {
            var user = await this.userManager.GetUserAsync(User);
            await this.emailService.WorkInProcessAsync(user, messageId);
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var body = mailDTO.BodyAsString;

            return Json(body);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ValidateMail(string messageId)
        {
            var user = await this.userManager.GetUserAsync(User);
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var validStatus = await this.statusService.GetStatusByName("New");
            await emailService.ValidateEmail(mail, mailDTO.BodyAsString, validStatus,user);
            return Ok();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            logger.LogInformation("I am in about");
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ContactManager() //available to anyone Operators -> Manager Page, not logged or Managers -> Administrators contacts
        {
            var user = Environment.UserName;

            var managers = new List<ManagerViewModel>();
            foreach (var manager in await this.userService.GetManagersAsync())
            {
                managers.Add(new ManagerViewModel { UserName = manager.UserName, Email = manager.Email });
            }


            return View(managers);
        }

        // [Authorize(Roles = "Manager, Operator")]
        public async Task<IActionResult> ListStatusNew(int? pageNumber) // Accessible to all logged users and managers to see
        {
            var mailStatusNewLst = this.emailService.ReadAllMailsFromDb().Where(e => e.Status.Text == "New");
            var pageSize = 10;
            var page = await PaginatedList<Email>.CreateAsync(mailStatusNewLst, pageNumber ?? 1, pageSize);

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> OpenApplication(string messageId) // Accessible to all logged users and managers to see New->Open
        {
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);


            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            var newStatus = await this.statusService.GetStatusAsync("Open");
            var email = await this.emailService.GetEmailByIdAsync(2);
            await auditLogService.Log(userName, "CHANGED STATUS", newStatus, email.Status); // Audit logs => how to display action? One more type i db or strings, user?
            email = await this.emailService.UpdateStatusAsync(email, newStatus);
            var model = this.emailViewModelMapper.MapFrom(email);




            var validStatus = await this.statusService.GetStatusByName("New");
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetBodyDB(string messageId)
        {
            var body = await this.emailService.GetEmailBodyAsync(messageId);
            var res = Json(body);

            return Json(body);
        }
        //status open, work in process

        public async Task<IActionResult> ChangeStatusToOpen(string messageId)
        {
            var mail = await emailService.GetEmailByIdDBAsync(messageId);
            mail.Status = await this.statusService.GetStatusAsync("Open");
            //mail.WorkInProccess()

        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
