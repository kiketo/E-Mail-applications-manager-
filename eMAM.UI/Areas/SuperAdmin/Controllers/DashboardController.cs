using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.DTO;
using eMAM.Service.UserServices.Contracts;
using eMAM.UI.Areas.SuperAdmin.Models;
using eMAM.UI.Mappers;
using eMAM.UI.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//TODO to finalize
namespace eMAM.UI.Areas.SuperAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IGmailUserDataService gmailUserDataService;
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly IUserViewModelMapper<User, UserViewModel> userMapper;

        public DashboardController(IGmailUserDataService gmailUserDataService, IUserService userService, UserManager<User> userManager, IUserViewModelMapper<User, UserViewModel> userMapper)
        {
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
        }

        public IActionResult Index()
        {
            return View();
        }

        [Area("SuperAdmin")]
        [Authorize(Roles = "SuperAdmin")]
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
                //var userData = new GmailUserData
                //{
                //    AccessToken = userDataDTO.AccessToken,
                //    RefreshToken = userDataDTO.RefreshToken,
                //    ExpiresAt = DateTime.Now.AddSeconds(userDataDTO.ExpiresInSec)
                //};

                await this.gmailUserDataService.CreateAsync(userDataDTO);

                return Redirect("superadmin");
            }
        }

        [Area("SuperAdmin")]
        [Route("superadmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users(int? pageNumber)
        {
            var pageSize = 10;
            var users = this.userService.GetAllUsersQuery();
            var page = await PaginatedList<User>.CreateAsync(users, pageNumber ?? 1, pageSize);
            page.Reverse();

            UserViewModel model = new UserViewModel
            {
                HasNextPage = page.HasNextPage,
                HasPreviousPage = page.HasPreviousPage,
                PageIndex = page.PageIndex,
                TotalPages = page.TotalPages,
            };

            foreach (var user in page)
            {
                var element = await this.userMapper.MapFrom(user);
                model.UsersList.Add(element);
            }

            return View(model);



            //var users = await this.userService.GetAllUsersAsync();
            //UserViewModel model = new UserViewModel();
            //model.UsersList = (users)
            //       .Select(this.userMapper.MapFrom)
            //       .ToList();
            //foreach (var user in model.UsersList)
            //{
            //    user.Roles = new List<string>();
            //    user.Roles = await userManager.GetRolesAsync(user.User);
            //}
            //return View(model);
        }

        [Area("SuperAdmin")]
        [Route("superadmin/ChangeUserRole")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleRoleBetweenUserManager(string userId)
        {
            try
            {
                var user = await this.userService.ToggleRoleBetweenUserManagerAsync(userId);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        //public async Task<bool> TryReadGmail(HttpClient client, GmailUserDataDTO userDataDTO)
        //{
        //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userDataDTO.AccessToken);

        //    var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages");

        //    var content = await res.Content.ReadAsStringAsync();

        //    return res.IsSuccessStatusCode;
        //}
    }
}