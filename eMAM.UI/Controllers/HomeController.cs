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

namespace eMAM.UI.Controllers
{
    public class HomeController : Controller
    {
        private IGmailApiService gmailApiService;
        private GmailService service;

        public HomeController(IGmailApiService gmailApiService, GmailService service)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetMails ()
        {

            var msgs=this.gmailApiService.ListMessages(this.service, "me");
            foreach (var item in msgs)
            {
                Console.WriteLine(item.Id);
            }
            return View();
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
