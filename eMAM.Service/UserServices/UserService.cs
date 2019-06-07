using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.UserServices.Contracts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.UserServices
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<User>> GetManagersAsync()
        {

            var managerRoleId = await this.context.Roles.FirstOrDefaultAsync(m => m.Name == "Manager");
            var roleUserIds = await this.context.UserRoles.Where(m => m.RoleId == managerRoleId.Id).ToListAsync();
            var managers = new List<User>();
            foreach (var pair in roleUserIds)
            {
                managers.Add(await GetUserByIdAsync(pair.UserId));
            }
            return managers;
        }
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await this.context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
