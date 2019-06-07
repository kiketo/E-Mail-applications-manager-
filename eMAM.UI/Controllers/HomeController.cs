﻿using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.GmailServices.Contracts;
using eMAM.UI.Mappers;
using eMAM.UI.Models;
using eMAM.UI.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public HomeController(IGmailApiService gmailApiService, IGmailUserDataService gmailUserDataService, IEmailService emailDbService, IViewModelMapper<Email, EmailViewModel> emailViewModelMapper)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.emailService = emailDbService ?? throw new ArgumentNullException(nameof(emailDbService));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
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
            var mail = await this.emailService.GetEmailByGmailIdAsync(messageId);
            var body = mailDTO.BodyAsString;
            //var model = this.emailViewModelMapper.MapFrom(mail);
            var res = Json(body);

            return Json(body);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult FindAdmin()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
