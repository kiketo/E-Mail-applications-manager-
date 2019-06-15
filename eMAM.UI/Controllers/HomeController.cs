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
using Microsoft.EntityFrameworkCore;
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
        private readonly ICustomerService customerService;

        public HomeController(UserManager<User> userManager,
            IGmailApiService gmailApiService,
            IGmailUserDataService gmailUserDataService,
            IEmailService emailService,
            IViewModelMapper<Email, EmailViewModel> emailViewModelMapper,
            IUserService userService,
            IAuditLogService auditLogService,
            IStatusService statusService,
            ILogger<User> logger,
            ICustomerService customerService)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }



        [Authorize] // MAnager & Operator
        public async Task<IActionResult> Index()
        {
            var isManager = User.IsInRole("Manager");
            var user = await this.userManager.GetUserAsync(User);
            var mails = this.emailService.ReadAllMailsFromDb(isManager, user);
            var model = new HomeViewModel();
            List<Email> listEmails = await this.emailService.ReadAllMailsFromDb(true, user).ToListAsync();
            var modelEmails = listEmails.Select(x => emailViewModelMapper.MapFrom(x));
            model.NotReviewed = modelEmails.Where(s => s.Status.Text == "Not Reviewed").OrderByDescending(x => x.DateReceived).Take(5).ToList();
            model.New = modelEmails.Where(s => s.Status.Text == "New").OrderByDescending(x => x.DateReceived).Take(5).ToList();

            if (!User.IsInRole("Manager"))
            {
                model.Open = modelEmails.Where(s => s.Status.Text == "Open").Where(u => u.WorkingBy == user).OrderByDescending(x => x.DateReceived).Take(5).ToList();
                model.Closed = modelEmails.Where(s => s.Status.Text == "Aproved" || s.Status.Text == "Rejected").OrderBy(x => x.DateReceived).Take(5).ToList();
                model.UserIsManager = isManager;
                return View(model);
            }
            model.Open = modelEmails.Where(s => s.Status.Text == "Open").OrderByDescending(x => x.DateReceived).Take(5).ToList();
            model.Closed = modelEmails.Where(s => s.Status.Text == "Aproved" || s.Status.Text == "Rejected").OrderBy(x => x.DateReceived).Take(5).ToList();
            return View(model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ListAllMails2(int? pageNumber)
        {
            var pageSize = 10;
            var user = await this.userManager.GetUserAsync(User);
            var isManager = User.IsInRole("Manager");
            var mails = this.emailService.ReadAllMailsFromDb(isManager, user);
            var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);
            page.Reverse();

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
                UserIsManager = isManager
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }

            return View(model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ListAllMails(int? pageNumber, bool currentFilter = false, bool newFilter = false)
        {
            if (newFilter)
            {
                pageNumber = 1;
            }
            else
            {
                newFilter = currentFilter;
            }

            //ViewData["CurrentFilter"] = newFilter;

            var pageSize = 10;
            var user = await this.userManager.GetUserAsync(User);
            var isManager = User.IsInRole("Manager");
            var mails = this.emailService.ReadAllMailsFromDb(isManager, user);
            if (newFilter)
            {
                mails = mails.Where(e => e.Status.Text == "Invalid Application");
            }
            var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);
            page.Reverse();

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
                UserIsManager = isManager,
                FilterOnlyNotValid = newFilter
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }

            return View(model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ListOpenEmails(int? pageNumber)
        {
            var pageSize = 10;
            var user = await this.userManager.GetUserAsync(User);
            var isManager = User.IsInRole("Manager");
            var mails = this.emailService.ReadOpenMailsFromDb(isManager, user);
            var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);
            //page.Reverse();

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
                UserIsManager = isManager
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                element.InCurrentStatusSince = DateTime.Now - element.SetInCurrentStatusOn;
                model.SearchResults.Add(element);
            }

            return View(model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ListClosedMails(int? pageNumber, string currentFilterUser = null, string newFilterUser
            = null)
        {
            var pageSize = 10;
            var user = await this.userManager.GetUserAsync(User);
            var isManager = User.IsInRole("Manager");
            var mails = this.emailService.ReadAllMailsFromDb(isManager, user);
            if (String.IsNullOrEmpty(newFilterUser))
            {
                pageNumber = 1;
                mails = mails.Where(e => e.Status.Text == "Invalid Application");
            }
            else
            {
                newFilterUser = currentFilterUser;
            }

            var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
                UserIsManager = isManager,
                FilterByUser = newFilterUser
            };

            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PreviewMail(string messageId)
        {
            var user = await this.userManager.GetUserAsync(User);
              await this.emailService.WorkInProcessAsync(user, messageId);//TODO stops PREVIEW
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var body = mailDTO.BodyAsString;

            return Json(body);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ValidateMail(string messageId)
        {
            var user = await this.userManager.GetUserAsync(User);
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var validStatus = await this.statusService.GetStatusByName("New");
            //await this.auditLogService.Log(user.UserName, "status change", messageId, validStatus.Text, mail.Status.Text);
            mail.Status = validStatus;
            mail.Body = mailDTO.BodyAsString;
            mail.WorkInProcess = false;
            await emailService.UpdateAsync(mail);

            var model = this.emailViewModelMapper.MapFrom(mail);
            model.UserIsManager = User.IsInRole("Manager");

            //await emailService.ValidateEmail(mail, mailDTO.BodyAsString, validStatus,user);
            //await emailService.WorkNotInProcessAsync(messageId);
            return PartialView("_AllEmailsPartial", model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NotValidMail(string messageId)
        {
            var user = await this.userManager.GetUserAsync(User);
            //var userData = await this.gmailUserDataService.GetAsync();
            //var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var invalidStatus = await this.statusService.GetStatusByName("Invalid Application");
            mail.Status = invalidStatus;
            mail.WorkInProcess = false;
            mail.WorkingBy = null;
            mail.SetInTerminalStatusOn = DateTime.Now;
            await emailService.UpdateAsync(mail);
            //await emailService.WorkNotInProcessAsync(messageId);
            var model = this.emailViewModelMapper.MapFrom(mail);
            model.UserIsManager = User.IsInRole("Manager");

            return PartialView("_AllEmailsPartial", model);
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NotPreviewed(string id)
        {
            var user = await this.userManager.GetUserAsync(User);
            //var userData = await this.gmailUserDataService.GetAsync();
            //var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var mail = await this.emailService.GetEmailByGmailIdAsync(id);
            var notPreviewedStatus = await this.statusService.GetInitialStatusAsync();
            mail.Status = notPreviewedStatus;
            mail.WorkInProcess = false;
            mail.WorkingBy = null;
            await emailService.UpdateAsync(mail);
            //await emailService.WorkNotInProcessAsync(messageId);
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



        [Authorize]
        public async Task<IActionResult> ListStatusNew(int? pageNumber) // Accessible to all logged users and managers to see
        {
            var pageSize = 10;
            var isManager = User.IsInRole("Manager");
            var user = await this.userManager.GetUserAsync(User);
            var mailStatusNewLst = this.emailService.ReadAllMailsFromDb(isManager, user).Where(e => e.Status.Text == "New");
            var page = await PaginatedList<Email>.CreateAsync(mailStatusNewLst, pageNumber ?? 1, pageSize);
            page.Reverse();

            EmailViewModel model = new EmailViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
                UserIsManager = isManager
            };
            foreach (var mail in page)
            {
                var element = this.emailViewModelMapper.MapFrom(mail);
                model.SearchResults.Add(element);
            }
            return View(model);
        }


        //[HttpPost]
        //public async Task<IActionResult> OpenApplication(string messageId) // Accessible to all logged users and managers to see New->Open
        //{
        //    var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);


        //   string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //    var newStatus = await this.statusService.GetStatusAsync("Open");
        //    var email = await this.emailService.GetEmailByIdAsync(2);
        //    await auditLogService.Log(userName, "CHANGED STATUS", newStatus, email.Status); // Audit logs => how to display action? One more type i db or strings, user?
        //    await this.emailService.UpdateAsync(email);
        //    var model = this.emailViewModelMapper.MapFrom(email);
        //    var validStatus = await this.statusService.GetStatusByName("New");
        //    return Ok();
        //}

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetBodyDB(string messageId) // changes status to Open and working TODO put auditlog
        {
            var body = await this.emailService.GetEmailBodyAsync(messageId);
            var email = await this.emailService.GetEmailByGmailIdAsync(messageId);
            email.Status = await this.statusService.GetStatusAsync("Open");
            email.WorkInProcess = true;
            email.WorkingBy = await userManager.GetUserAsync(User);
            await this.emailService.UpdateAsync(email);

            var res = Json(body);

            return Json(body);
        }

        //status open, work in process
        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeStatusToOpen(string messageId)
        {
            var email = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var user = await this.userManager.GetUserAsync(User);
            var newStatus = await this.statusService.GetStatusAsync("Open");
            //await this.auditLogService.Log(user.UserName, "status change", messageId, newStatus.Text, email.Status.Text);
            
            var mail = await emailService.GetEmailByGmailIdAsync(messageId);
            mail.Status = newStatus;
            mail.WorkInProcess = true;
            mail.WorkingBy = user;
            mail.OpenedBy = user;
            mail.SetInCurrentStatusOn = DateTime.Now;
            await this.emailService.UpdateAsync(mail);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNCloseApplicationAproved(EmailViewModel model)
        {

            if (ModelState.IsValid)
            {
                var email = await this.emailService.GetEmailByGmailIdAsync(model.GmailIdNumber);
                Customer customer;
                if (await this.customerService.GetCustomerByEGNAsync(model.CustomerEGN) == null)
                {
                    customer = await this.customerService.CreateNewCustomerAsync(model.CustomerEGN, model.CustomerPhoneNumber);
                    customer.Emails.Add(email);
                }
                else
                {
                    customer = await this.customerService.GetCustomerByEGNAsync(model.CustomerEGN);
                    customer.Emails.Add(email);
                }
                email.Status = await this.statusService.GetStatusAsync("Aproved");
                email.ClosedBy = await this.userManager.GetUserAsync(User);
                email.WorkInProcess = false;
                await this.emailService.UpdateAsync(email);
                return Ok();
            }

            return BadRequest();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SubmitNCloseApplicationRejected(EmailViewModel model)
        {
            var email = await this.emailService.GetEmailByGmailIdAsync(model.GmailIdNumber);

            email.Status = await this.statusService.GetStatusAsync("Rejected");
            email.ClosedBy = await this.userManager.GetUserAsync(User);
            email.WorkInProcess = false;
            await this.emailService.UpdateAsync(email);
            return Ok();
        }


        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ManagerStatusChangeNotReviewed(string messageId)
        {
            var email = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var user = await this.userManager.GetUserAsync(User);

            var newStatus = await this.statusService.GetInitialStatusAsync();

            await this.auditLogService.Log(user.UserName, "status change", email.GmailIdNumber, newStatus.Text, email.Status.Text);
            email.Status = newStatus;
            email.ClosedBy = null;
            email.WorkingBy = null;
            email.WorkInProcess = false;
            email.Body = null;
            await this.emailService.UpdateAsync(email);
            return Ok();
        }

        //[Authorize(Roles = "Manager, Operator")]
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public async Task<IActionResult> StatusChangeNew(string messageId) // manager open new application - status change
        //{
        //    var email = await this.emailService.GetEmailByGmailIdAsync(messageId);
        //    var user = await this.userManager.GetUserAsync(User);

        //    var newStatus = await this.statusService.GetStatusAsync("New");

        //    await this.auditLogService.Log(user.UserName, "status change", email.GmailIdNumber, newStatus.Text, email.Status.Text);
        //    email.Status = newStatus;
        //    email.ClosedBy = null;
        //    email.WorkingBy = user;
        //    email.WorkInProcess = true;
        //    email.OpenedBy = user;
        //    await this.emailService.UpdateAsync(email);
        //    return Ok();
        //}




        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
