using eMAM.Data.Models;
using System.Threading.Tasks;

namespace eMAM.Service.Contracts
{
    public interface IGmailUserDataService
    {
        Task CreateAsync(GmailUserData gmailUserData);

        Task UpdateAsync(GmailUserData gmailUserData);

        GmailUserData Get();
    }
}
