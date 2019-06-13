using eMAM.Data.Models;
using eMAM.UI.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Areas.SuperAdmin.Models
{
    public class UserViewModelMapper : IViewModelMapper<User, UserViewModel>
    {
        public UserViewModel MapFrom(User entity)

       => new UserViewModel
       {
           Id = entity.Id,
           UserName = entity.UserName,
           User = entity,
            //RolesList = await this._userManager.GetRolesAsync(entity),
            Email = entity.Email
       };
    }
}
