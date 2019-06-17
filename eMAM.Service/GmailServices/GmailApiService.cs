using Crypteron.CipherObject;
using eMAM.Service.DTO;
using eMAM.Service.GmailServices.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.GmailServices
{
    public class GmailApiService : IGmailApiService
    {
        //private readonly ApplicationDbContext context;
        //private readonly string userId = "me";

        //public GmailApiService(ApplicationDbContext context)
        //{
        //    this.context = context ?? throw new ArgumentNullException(nameof(context));
        //}

        public async Task<GmailUserDataDTO> RenewAccessTokenAsync(string refreshToken)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("client_id","667407283017-hjbtv4a5sjr3garaprkidqo36qs4u7o3.apps.googleusercontent.com"),
                new KeyValuePair<string,string>("client_secret","cH5tErPh_uqDZDmp1F1aDNIs"),
                new KeyValuePair<string,string>("refresh_token",refreshToken),//.Unseal()),
                new KeyValuePair<string,string>("grant_type","refresh_token"),
            });

            var res = await client.PostAsync("https://oauth2.googleapis.com/token", content);

            if (!res.IsSuccessStatusCode)
            {
                throw new ArgumentException("Access token did not refresh correctly!");
            }
            else
            {
                var userDataDTO = JsonConvert.DeserializeObject<GmailUserDataDTO>(await res.Content.ReadAsStringAsync());
                return userDataDTO;
            }
        }

        public async Task<GmailMessagesListDTO> DownloadMailsListAsync(string AccessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);//.Unseal());

            var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages?includeSpamTrash=true&maxResults=999999");

            var content = await res.Content.ReadAsStringAsync();
            var gmailMessagesList = JsonConvert.DeserializeObject<GmailMessagesListDTO>(await res.Content.ReadAsStringAsync());

            return gmailMessagesList;
        }

        public async Task<GmailMessageDTO> DownloadMail(string messageId, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);//.Unseal());

            var str = new StringBuilder("https://www.googleapis.com/gmail/v1/users/me/messages/")
                    .Append(messageId)
                    .Append("?format=full");
            var res = await client.GetAsync(str.ToString());
            var content = await res.Content.ReadAsStringAsync();

            var firstMessage = JsonConvert.DeserializeObject<GmailMessageDTO>(await res.Content.ReadAsStringAsync());

            return firstMessage;
        }

        //public async Task DownloadNewMailsJson(GmailUserData userData)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userData.AccessToken);

        //    var res = await client.GetAsync("https://www.googleapis.com/gmail/v1/users/me/messages?includeSpamTrash=true");

        //    var content = await res.Content.ReadAsStringAsync();
        //    var gmailMessagesList = JsonConvert.DeserializeObject<GmailMessagesListDTO>(await res.Content.ReadAsStringAsync());

        //    int countNewMails = gmailMessagesList.Count - await this.context.Emails.CountAsync();

        //    var status = await this.context.Statuses.FirstOrDefaultAsync(s => s.Text == "Not Reviewed");

        //    while (true)
        //    {
        //        if (countNewMails == 0)
        //        {
        //            break;
        //        }
        //        var messageId = gmailMessagesList.Messages.First().Id;
        //        var messageAlreadyInDb = await this.context.Emails.AnyAsync(e => e.GmailIdNumber == messageId);
        //        if (messageAlreadyInDb)
        //        {
        //            gmailMessagesList.Messages.RemoveAt(0);
        //            continue;
        //        }
        //        var str = new StringBuilder("https://www.googleapis.com/gmail/v1/users/me/messages/")
        //            .Append(messageId)
        //            .Append("?format=full");
        //        res = await client.GetAsync(str.ToString());
        //        content = await res.Content.ReadAsStringAsync();

        //        var firstMessage = JsonConvert.DeserializeObject<GmailMessageDTO>(await res.Content.ReadAsStringAsync());

        //        var headers = firstMessage.Payload.Headers;

        //        //Gmail epoch time in ms
        //        var gmail_date = (long)firstMessage.InternalDate;

        //        //Get DateTime of epoch ms
        //        var to_date = DateTimeOffset.FromUnixTimeMilliseconds(gmail_date).DateTime;

        //        //This is your timezone offset GMT +3
        //        var offset = 3;

        //        var date = to_date - new TimeSpan(offset * -1, 0, 0);

        //        var subject = headers.FirstOrDefault(s => s.Name == "Subject").Value;
        //        var senderFull = headers.FirstOrDefault(s => s.Name == "From").Value;
        //        var senderMail = senderFull.Substring(senderFull.IndexOf('<') + 1).Replace('>', ' ').TrimEnd();
        //        var index = senderFull.IndexOf('<');
        //        var senderName = senderFull.Substring(0, index - 1);

        //        Sender sender = await this.context.Senders.FirstOrDefaultAsync(s => s.SenderEmail == senderMail && s.SenderName == senderName);
        //        if (sender == null)
        //        {
        //            sender = new Sender { SenderEmail = senderMail, SenderName = senderName };
        //            await this.context.Senders.AddRangeAsync(sender);
        //            await this.context.SaveChangesAsync();
        //        }

        //        List<Attachment> attachments = new List<Attachment>();
        //        if (firstMessage.Payload.MimeType == "multipart/mixed")
        //        {
        //            var partsWithAtt = firstMessage.Payload.Parts.Skip(1);
        //            foreach (var part in partsWithAtt)
        //            {
        //                Attachment newAttachment = new Attachment
        //                {
        //                    FileName = part.FileName,
        //                    FileSizeInMb = (double)part.Body.SizeInBytes / (1024 * 1024)
        //                };
        //                attachments.Add(newAttachment);
        //                await this.context.Attachments.AddAsync(newAttachment);
        //            }
        //        }
        //        Email newMail = new Email
        //        {
        //            DateReceived = date,
        //            Attachments = attachments,
        //            GmailIdNumber = messageId,
        //            InitialRegistrationInSystemOn = DateTime.Now,
        //            SetInCurrentStatusOn = DateTime.Now,
        //            Sender = sender,
        //            Status = status,
        //            Subject = subject
        //        };

        //        await this.context.Emails.AddAsync(newMail);
        //        await this.context.SaveChangesAsync();
        //        gmailMessagesList.Messages.RemoveAt(0);
        //        countNewMails--;
        //    }
        //}

        public async Task<GmailMessageDTO> DownloadBodyOfMailAsync(string messageId, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);//.Unseal());
            var str = new StringBuilder("https://www.googleapis.com/gmail/v1/users/me/messages/")
                   .Append(messageId)
                   .Append("?format=full");
            var res = await client.GetAsync(str.ToString());
            var content = await res.Content.ReadAsStringAsync();

            var message = JsonConvert.DeserializeObject<GmailMessageDTO>(await res.Content.ReadAsStringAsync());


            // var message = this.service.Users.Messages.Get(userId, messageId).Execute();

            switch (message.Payload.MimeType)
            {
                //plain text, no attachments
                case "text/plain":
                case "text/html":
                    message = ParsePlainTextMail(message);
                    break;

                //html, no attachments
                case "multipart/alternative":
                    message = ParseHtmlMail(message);
                    break;

                //plain/html text, with attachments
                case "multipart/mixed":
                    message = ParseMailWithAttachments(message);
                    break;

                //html text, no attachments
                //case "text/html":
                //    message = ParseMailWithAttachments(message);
                //    break;
                default:
                    break;
            }
            return message;
        }

        private GmailMessageDTO ParseMailWithAttachments(GmailMessageDTO message)
        {
            string mailData;
            if (message.Payload.Parts[0].Parts != null)
            {
                mailData = message.Payload.Parts[0].Parts[1].Body.Data;
            }
            else
            {
                mailData = message.Payload.Parts[0].Body.Data;
            }

            message.BodyAsString = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));
            return message;
        }

        private GmailMessageDTO ParseHtmlMail(GmailMessageDTO message)
        {

            string mailData;// = message.Payload;//.Parts[1].Body.Data;
            if (message.Payload.Parts.Count==2)
            {
                mailData = message.Payload.Parts[1].Body.Data;
            }
            else
            {
                mailData = message.Payload.Parts[0].Body.Data;
            }
            message.BodyAsString = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));

            return message;
        }

        private GmailMessageDTO ParsePlainTextMail(GmailMessageDTO message)
        {
            var mailData = message.Payload.Body.Data;
            message.BodyAsString = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));

            return message;
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
