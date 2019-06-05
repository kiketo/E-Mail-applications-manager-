using eMAM.Data.Models;
using eMAM.Service.DTO;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IGmailUserDataService
    {
        Task CreateAsync(GmailUserDataDTO gmailUserData);

        Task UpdateAsync(GmailUserDataDTO gmailUserData);

        Task<GmailUserData> GetAsync();

        Task<bool> IsAccessTokenValidAsync();
    }
}
