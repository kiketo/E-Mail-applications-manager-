using System.Collections.Generic;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace eMAM.Service.Contracts
{
    public interface IGmailApiService
    {
        List<Message> ListMessages(GmailService service, string userId);
    }
}