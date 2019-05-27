using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eMAM.UI.Models;
using eMAM.Service.Contracts;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using eMAM.UI.Mappers;
using eMAM.Data.Models;

namespace eMAM.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGmailApiService gmailApiService;
        private readonly GmailService service;
        private readonly IViewModelMapper<Email, EmailViewModel> emailViewModelMapper;

        public HomeController(IGmailApiService gmailApiService, GmailService service, IViewModelMapper<Email, EmailViewModel> emailViewModelMapper)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetMails ()
        {

            await this.gmailApiService.DownloadNewMailsWithoutBodyAsync();

            var mails = await this.gmailApiService.ReadAllMailsFromDbAsync();
            EmailViewModel model = new EmailViewModel();     

            foreach (var mail in mails)
            {
                model.SearchResults.Add(this.emailViewModelMapper.MapFrom(mail));
            }

            //foreach (var item in msgs)
            //{
            //    Console.WriteLine(item.Id);
            //}
            return View(model);
        }







        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
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
