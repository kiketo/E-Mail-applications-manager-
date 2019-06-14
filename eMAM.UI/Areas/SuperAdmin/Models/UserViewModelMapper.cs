using eMAM.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace eMAM.UI.Areas.SuperAdmin.Models
{
    public class UserViewModelMapper : IUserViewModelMapper<User, UserViewModel>
    {
        private readonly UserManager<User> userManager;

        public UserViewModelMapper(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<UserViewModel> MapFrom(User entity)
        {
            var res = new UserViewModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                User = entity,
                Roles = await this.userManager.GetRolesAsync(entity),
                Email = entity.Email
            };
            return res;
        }
    }
}
