using eMAM.Data.Models;
using eMAM.Service.Contracts;
using eMAM.Service.DTO;
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
        //private readonly IGmailApiService gmailApiService;
        //private readonly GmailService service;
        private IViewModelMapper<Email, EmailViewModel> emailViewModelMapper;
        private readonly IGmailUserDataService gmailUserDataService;

        public HomeController(IViewModelMapper<Email, EmailViewModel> emailViewModelMapper, IGmailUserDataService gmailUserDataService)
        {
            //this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
           // this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.emailViewModelMapper = emailViewModelMapper ?? throw new ArgumentNullException(nameof(emailViewModelMapper));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GoogleLogin()
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
                var userDataDTO = JsonConvert.DeserializeObject<GmailUserDataDTO>(await res.Content.ReadAsStringAsync());
                var userData = new Data.Models.GmailUserData
                {
                    AccessToken = userDataDTO.AccessToken,
                    RefreshToken = userDataDTO.RefreshToken,
                    ExpiresAt = DateTime.Now.AddSeconds(userDataDTO.ExpiresInSec)
                };

                await this.gmailUserDataService.CreateAsync(userData);

                return Json(await TryReadGmail(client, userData));
            }
        }

        public async Task<bool> TryReadGmail (HttpClient client, Data.Models.GmailUserData userData)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userData.AccessToken);

            var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages");

            var content = await res.Content.ReadAsStringAsync();

            return res.IsSuccessStatusCode;
        }

        //public async Task<IActionResult> RenewAccesToken()
        //{
        //    await this.gmailApiService.RenewAccessTokenAsync();
        //    var res = this.gmailUserDataService.Get();
        //    return Json(res.ExpiresAt);
        //}

        //public async Task<IActionResult> GetMails()
        //{
        //    await this.gmailApiService.DownloadNewMailsWithoutBodyAsync();

        //    //EmailViewModel model = new EmailViewModel();     

        //    return View();
        //}

        //public async Task<IActionResult> ListMails(int? pageNumber)
        //{

        //    var mails = this.gmailApiService.ReadAllMailsFromDb();


        //    var pageSize = 10;

        //    var page = await PaginatedList<Email>.CreateAsync(mails, pageNumber ?? 1, pageSize);

        //    EmailViewModel model = new EmailViewModel
        //    {
        //        HasNextPage = page.HasNextPage,
        //        HasPreviousPage = page.HasPreviousPage,
        //        PageIndex = page.PageIndex,
        //        TotalPages = page.TotalPages
        //    };

        //    foreach (var mail in page)
        //    {
        //        var element = this.emailViewModelMapper.MapFrom(mail);
        //        model.SearchResults.Add(element);
        //    }

        //    return View(model);
        //}

        public async Task<IActionResult> DownloadMailsAsJson()
        {
            var accessToken = this.gmailUserDataService.Get().AccessToken;
            var client = new HttpClient();
            var header = new StringBuilder()
                .Append("https://www.googleapis.com/gmail/v1/users/me/messages?access_token=")
                .Append(accessToken);
            var res=await client.GetAsync(header.ToString());










            return Json(res.IsSuccessStatusCode);


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
