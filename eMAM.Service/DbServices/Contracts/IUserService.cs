using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.UserServices.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetManagersAsync();

        Task<User> GetUserByIdAsync(string id);

        IQueryable<User> GetAllUsersQuery();

        Task<User> ToggleRoleBetweenUserManagerAsync(string userId);

        Task<User> ToggleRoleBetweenUserOperatorAsync(string userId);
    }
}
