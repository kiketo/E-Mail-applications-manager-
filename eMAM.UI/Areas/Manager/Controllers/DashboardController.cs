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
namespace eMAM.UI.Areas.Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly IUserViewModelMapper<User, UserViewModel> userMapper;

        public DashboardController(IUserService userService, UserManager<User> userManager, IUserViewModelMapper<User, UserViewModel> userMapper)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
        }

        public IActionResult Index()
        {
            return View();
        }

        [Area("Manager")]
        [Route("manager")]
        [Authorize(Roles = "Manager")]
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
        }

        [Area("Manager")]
        [Route("manager/ChangeUserRole")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ToggleRoleBetweenUserOperator(string userId)
        {
            try
            {
                var user = await this.userService.ToggleRoleBetweenUserOperatorAsync(userId);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}