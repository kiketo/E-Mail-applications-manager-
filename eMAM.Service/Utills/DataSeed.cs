using eMAM.Data;
using eMAM.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.Service.Utills
{
    public static class DataSeed
    {
        public static async Task SeedDatabaseWithSuperAdminAsync(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dbContext.Roles.Any(u => u.Name == "SuperAdmin"))
                {
                    return;
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await rolemanager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
                await rolemanager.CreateAsync(new IdentityRole { Name = "Admin" });
                await rolemanager.CreateAsync(new IdentityRole { Name = "Operator" });
                await rolemanager.CreateAsync(new IdentityRole { Name = "Manager" });

                var superAdminUser = new User { UserName= "super@admin.user", Email = "super@admin.user" };
                await userManager.CreateAsync(superAdminUser, "Admin123!");
                await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
            }
        }

        public static async Task SeedDatabaseWithStatus(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dbContext.Statuses.Any())
                {
                    return;
                }

                Status notReviewed = new Status
                {
                    Text = "Not Reviewed"
                };
                Status invalidApplication = new Status
                {
                    Text = "Invalid Application"
                };
                Status newStat = new Status
                {
                    Text = "New"
                };
                Status open = new Status
                {
                    Text = "Open"
                };
                Status aproved = new Status
                {
                    Text = "Aproved"
                };
                Status rejected = new Status
                {
                    Text = "Rejected"
                };

                await dbContext.Statuses.AddAsync(notReviewed);
                await dbContext.Statuses.AddAsync(invalidApplication);
                await dbContext.Statuses.AddAsync(newStat);
                await dbContext.Statuses.AddAsync(open);
                await dbContext.Statuses.AddAsync(aproved);
                await dbContext.Statuses.AddAsync(rejected);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
