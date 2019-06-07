using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.GmailServices.Contracts;
using eMAM.Service.UserServices.Contracts;
using eMAM.UI.Mappers;
using eMAM.UI.Models;
using eMAM.UI.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace eMAM.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGmailApiService gmailApiService;
        private readonly IGmailUserDataService gmailUserDataService;
        private readonly IEmailService emailService;
        private IViewModelMapper<Email, EmailViewModel> emailViewModelMapper;
        private readonly IUserService userService;


        public HomeController(
            IGmailApiService gmailApiService, 
            IGmailUserDataService gmailUserDataService, 
            IEmailService emailDbService, 
            IViewModelMapper<Email, EmailViewModel> emailViewModelMapper, 
            IUserService userService)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.emailService = emailDbService ?? throw new ArgumentNullException(nameof(emailDbService));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [Authorize]
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

        //[Authorize]
        //[HttpGet]
        //public  IActionResult PreviewMail()
        //{
        //    return View();
        //}

        //[ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PreviewMail(string messageId)
        {
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            //var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var body = mailDTO.BodyAsString;
            //var model = this.emailViewModelMapper.MapFrom(mail);
            var res = Json(body);

            return Json(body);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ValidateMail(string messageId)
        {
            var userData = await this.gmailUserDataService.GetAsync();
            var mailDTO = await this.gmailApiService.DownloadBodyOfMailAsync(messageId, userData.AccessToken);
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            //mail.Body = mailDTO.BodyAsString;
            await emailService.AddBodyToMailAsync(mail, mailDTO.BodyAsString);
            return Ok();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
       // [Authorize(Roles = "Operator")] //Create Areas
        public async Task<IActionResult> FindManager()
        {
            var managers = new List<ManagerViewModel>();
         //   managers.Add(new ManagerViewModel { UserName = "manager1", Email = "edsafd@fdsf.fsdf" });
            foreach (var manager in await this.userService.GetManagersAsync())
            {
                managers.Add(new ManagerViewModel { UserName = manager.UserName, Email = manager.Email });
            }

            //ViewData["Message"] = "Your contact page.";

            return View(managers);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
