using Crypteron.CipherObject;
using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class GmailUserDataService : IGmailUserDataService
    {
        private ApplicationDbContext context;

        public GmailUserDataService(ApplicationDbContext context)
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
            return await this.context.GmailUserData.FirstOrDefaultAsync();//.Unseal();
        }

        public async Task CreateAsync(GmailUserDataDTO gmailUserData)
        {
            GmailUserData userData = new GmailUserData
            {
                AccessToken = gmailUserData.AccessToken,
                RefreshToken = gmailUserData.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(gmailUserData.ExpiresInSec)
            };
            //userData.Seal();
            await this.context.GmailUserData.AddAsync(userData);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GmailUserDataDTO gmailUserData)
        {
            var userData = await this.context.GmailUserData.FirstOrDefaultAsync();
            userData.AccessToken = gmailUserData.AccessToken;
            userData.ExpiresAt = DateTime.Now.AddSeconds(gmailUserData.ExpiresInSec);
            //userData.Seal();
            this.context.Attach(userData).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}
