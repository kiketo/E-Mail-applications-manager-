using eMAM.Data.Models;
using eMAM.Service.Contracts;
using eMAM.UI.Mappers;
using eMAM.UI.Models;
using eMAM.UI.Utills;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGmailApiService gmailApiService;
        private readonly GmailService service;
        private IViewModelMapper<Email, EmailViewModel> emailViewModelMapper;

        public HomeController(IGmailApiService gmailApiService, GmailService service, IViewModelMapper<Email, EmailViewModel> emailViewModelMapper)
        {
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult GoogleLogin(string code)
        {
            var sb = new StringBuilder()
                .Append("https://accounts.google.com/o/oauth2/v2/auth?")
                .Append("scope=https://www.googleapis.com/auth/gmail.readonly")
                .Append("&access_type=offline")
                .Append("&include_granted_scopes=true")
                .Append("&response_type=code")
                .Append("&redirect_uri=http://localhost:5000/google-callback")
                .Append("&client_id=667407283017-hjbtv4a5sjr3garaprkidqo36qs4u7o3.apps.googleusercontent.com");

            //var url = HttpUtility.UrlEncode(sb.ToString());

            return Redirect(sb.ToString());
        }

        [Route("google-callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("code",code),
                new KeyValuePair<string,string>("client_id","667407283017-hjbtv4a5sjr3garaprkidqo36qs4u7o3.apps.googleusercontent.com"),
                new KeyValuePair<string,string>("client_secret","cH5tErPh_uqDZDmp1F1aDNIs"),
                new KeyValuePair<string,string>("redirect_uri","http://localhost:5000/google-callback"),
                new KeyValuePair<string,string>("grant_type","authorization_code"),
            });

            var res = await client.PostAsync("https://oauth2.googleapis.com/token", content);

            if (!res.IsSuccessStatusCode)
            {
                return Json(await res.Content.ReadAsStringAsync());
            }
            else
            {
                var userData = JsonConvert.DeserializeObject<UserData>(await res.Content.ReadAsStringAsync());
                return Json(await TryReadGmail(client, userData));
            }

        }

        public async Task<bool> TryReadGmail (HttpClient client, UserData userData)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userData.AccessToken);

            var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages");

            var content = await res.Content.ReadAsStringAsync();

            return res.IsSuccessStatusCode;
        }


        public class UserData
        {

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresInSec { get; set; }
        }



        public async Task<IActionResult> GetMails()
        {
            await this.gmailApiService.DownloadNewMailsWithoutBodyAsync();

            //EmailViewModel model = new EmailViewModel();     

            return View();
        }

        public async Task<IActionResult> ListMails(int? pageNumber)
        {

            var mails = this.gmailApiService.ReadAllMailsFromDb();


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
