using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using eMAM.Data.Models;
using eMAM.Service.DTO;
using eMAM.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//TODO to finalize
namespace eMAM.UI.Areas.SuperAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private GmailUserDataService gmailUserDataService;

        public DashboardController(GmailUserDataService gmailUserDataService)
        {
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
        }

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
                var userData = new GmailUserData
                {
                    AccessToken = userDataDTO.AccessToken,
                    RefreshToken = userDataDTO.RefreshToken,
                    ExpiresAt = DateTime.Now.AddSeconds(userDataDTO.ExpiresInSec)
                };

                await this.gmailUserDataService.CreateAsync(userData);

                return Json(await TryReadGmail(client, userData));
            }
        }
        public async Task<bool> TryReadGmail(HttpClient client, Data.Models.GmailUserData userData)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userData.AccessToken);

            var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages");

            var content = await res.Content.ReadAsStringAsync();

            return res.IsSuccessStatusCode;
        }
    }
}