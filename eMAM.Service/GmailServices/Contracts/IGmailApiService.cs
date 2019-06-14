using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eMAM.Data.Models;
using eMAM.Service.DTO;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace eMAM.Service.GmailServices.Contracts
{
    public interface IGmailApiService
    {
        Task<GmailMessageDTO> DownloadBodyOfMailAsync (string messageId, string accessToken);

        Task<GmailUserDataDTO> RenewAccessTokenAsync(string refreshToken);

        Task<GmailMessagesListDTO> DownloadMailsListAsync(string AccessToken);

        Task<GmailMessageDTO> DownloadMail(string messageId, string accessToken);
    }
}