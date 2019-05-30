using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eMAM.Data.Models;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace eMAM.Service.Contracts
{
    public interface IGmailApiService
    {
        Task DownloadNewMailsWithoutBodyAsync();

        Task DownloadBodyToMail(string messageId);

        IQueryable<Email> ReadAllMailsFromDb();
    }
}