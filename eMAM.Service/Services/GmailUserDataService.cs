using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.Service.Services
{
    public class GmailUserDataService : IGmailUserDataService
    {
        private ApplicationDbContext context;

        public GmailUserDataService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public GmailUserData Get()
        {
            return this.context.GmailUserData.FirstOrDefault();
        }

        public async Task CreateAsync(GmailUserData gmailUserData)
        {
            await this.context.GmailUserData.AddAsync(gmailUserData);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GmailUserData gmailUserData)
        {
            var userData = await this.context.GmailUserData.FirstOrDefaultAsync();
            userData.AccessToken = gmailUserData.AccessToken;
            userData.ExpiresAt = gmailUserData.ExpiresAt;

            await this.context.SaveChangesAsync();
        }
    }
}
