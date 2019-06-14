using eMAM.Data.Models;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public interface ISenderService
    {
        Task<Sender> GetSenderAsync(string senderMail, string senderName);

        Task<Sender> AddSenderAsync(string senderMail, string senderName);
    }
}