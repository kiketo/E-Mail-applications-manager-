using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.DTO;
using eMAM.Service.GmailServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class GmailUserDataService : IGmailUserDataService
    {
        private ApplicationDbContext context;

        public GmailUserDataService(ApplicationDbContext context, IGmailApiService gmailApiService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> IsAccessTokenValidAsync()
        {
            var userData = await this.context.GmailUserData.FirstOrDefaultAsync();
            if ((userData.ExpiresAt - DateTime.Now).TotalMinutes < 5)
            {
                return false;
            }
            return true;
        }

        public async Task<GmailUserData> GetAsync()
        {
            return await this.context.GmailUserData.FirstOrDefaultAsync();
        }

        public async Task CreateAsync(GmailUserDataDTO gmailUserData)
        {
            GmailUserData userData = new GmailUserData
            {
                AccessToken = gmailUserData.AccessToken,
                RefreshToken = gmailUserData.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(gmailUserData.ExpiresInSec)
            };

            await this.context.GmailUserData.AddAsync(userData);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GmailUserDataDTO gmailUserData)
        {
            var userData = await this.context.GmailUserData.FirstOrDefaultAsync();
            userData.AccessToken = gmailUserData.AccessToken;
            userData.ExpiresAt = DateTime.Now.AddSeconds(gmailUserData.ExpiresInSec);

            await this.context.SaveChangesAsync();
        }
    }
}
