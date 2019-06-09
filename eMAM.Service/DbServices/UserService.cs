using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.UserServices.Contracts;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;

        public UserService(ApplicationDbContext context, UserManager<User> userManager)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IEnumerable<User>> GetManagersAsync()
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
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await this.context.Users.ToListAsync();
        }
        public async Task<User> ChangeUserRoleAsync(string userId)
        {
            var updatedUser = await this.context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            var userRole = await this.userManager.GetRolesAsync(updatedUser);

            var allRoles = await this.context.Roles
                .Where(u => u.Name != "SuperAdmin")
                .ToListAsync();
            if (userRole[0] == allRoles[0].Name)
            {
                await this.userManager.RemoveFromRoleAsync(updatedUser, userRole[0]);
                await this.userManager.AddToRoleAsync(updatedUser, allRoles[1].Name);
            }
            else
            {
                await this.userManager.RemoveFromRoleAsync(updatedUser, userRole[0]);
                await this.userManager.AddToRoleAsync(updatedUser, allRoles[0].Name);
            }

            await this.context.SaveChangesAsync();

            return updatedUser;
        }
    }
}
