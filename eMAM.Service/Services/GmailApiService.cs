using eMAM.Data.Models;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eMAM.Service.Contracts
{
    public class GmailApiService : IGmailApiService
    {
        public List<Message> ListMessages(GmailService service, string userId)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);


            ListMessagesResponse response = request.Execute();
            result.AddRange(response.Messages);
            request.PageToken = response.NextPageToken;

            var firstMessage = service.Users.Messages.Get(userId, result.First().Id).Execute();
            var payload = new StringBuilder();
            foreach (var part in firstMessage.Payload.Parts)
            {
                byte[] data = FromBase64ForUrlString(part.Body.Data);
                string decodedString = Encoding.UTF8.GetString(data);
                payload.Append(decodedString);
            }


            var mailMessage = payload.ToString();


            return result;
        }

        private byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(string.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');

            return Convert.FromBase64String(result.ToString());
        }
    }
}
