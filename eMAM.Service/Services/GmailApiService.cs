using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DTO;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.Contracts
{
    public class GmailApiService : IGmailApiService
    {
        private readonly ApplicationDbContext context;
        private readonly IGmailUserDataService gmailUserDataService;
        private readonly GmailService service;
        private readonly string userId = "me";

        public GmailApiService(ApplicationDbContext context, IGmailUserDataService gmailUserDataService, GmailService service)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<GmailUserData> RenewAccessTokenAsync()
        {
            var refreshToken = this.context.GmailUserData
                                    .FirstOrDefault()
                                    .RefreshToken;

            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("client_id","667407283017-hjbtv4a5sjr3garaprkidqo36qs4u7o3.apps.googleusercontent.com"),
                new KeyValuePair<string,string>("client_secret","cH5tErPh_uqDZDmp1F1aDNIs"),
                new KeyValuePair<string,string>("refresh_token",refreshToken),
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
                var userData = new GmailUserData
                {
                    AccessToken = userDataDTO.AccessToken,
                    ExpiresAt = DateTime.Now.AddSeconds(userDataDTO.ExpiresInSec)
                };
                return userData;
            }
        }

        public async Task DownloadNewMailsWithoutBodyAsync()
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = this.service.Users.Messages.List(userId);

            ListMessagesResponse response = request.Execute();
            result.AddRange(response.Messages);
            request.PageToken = response.NextPageToken;
            List<Email> newMails = new List<Email>();
            var status = await this.context.Statuses.FirstOrDefaultAsync(s => s.Text == "Not Reviewed");

            List<Attachment> allAttachments = new List<Attachment>();
            List<Sender> allSenders = new List<Sender>();
            int countNewMails = result.Count - await this.context.Emails.CountAsync();

            while (true)
            {
                if (countNewMails == 0)
                {
                    break;
                }
                var messageId = result.First().Id;
                var firstMessage = this.service.Users.Messages.Get(userId, messageId).Execute();

                var messageAlreadyInDb = await this.context.Emails.AnyAsync(e => e.GmailIdNumber == messageId);
                if (messageAlreadyInDb)
                {
                    result.RemoveAt(0);
                    continue;
                }

                var headers = firstMessage.Payload.Headers;

                //Gmail epoch time in ms
                var gmail_date = (long)firstMessage.InternalDate;

                //Get DateTime of epoch ms
                var to_date = DateTimeOffset.FromUnixTimeMilliseconds(gmail_date).DateTime;

                //This is your timezone offset GMT +3
                var offset = 3;

                var date = to_date - new TimeSpan(offset * -1, 0, 0);

                //var date = DateTime.Parse(headers.FirstOrDefault(d => d.Name == "Date").Value);
                var subject = headers.FirstOrDefault(s => s.Name == "Subject").Value;
                var senderFull = headers.FirstOrDefault(s => s.Name == "From").Value;
                var senderMail = senderFull.Substring(senderFull.IndexOf('<') + 1).Replace('>', ' ').TrimEnd();
                var index = senderFull.IndexOf('<');
                var senderName = senderFull.Substring(0, index - 1);

                Sender sender = await this.context.Senders.FirstOrDefaultAsync(s => s.SenderEmail == senderMail && s.SenderName == senderName);
                if (sender == null)
                {
                    sender = new Sender { SenderEmail = senderMail, SenderName = senderName };
                    //if (allSenders.TrueForAll(s => s.SenderEmail != senderMail && s.SenderName != senderName))
                    //{
                    //allSenders.Add(sender);
                    await this.context.Senders.AddRangeAsync(sender);
                    await this.context.SaveChangesAsync();
                    // }
                }

                List<Attachment> attachments = new List<Attachment>();
                if (firstMessage.Payload.MimeType == "multipart/mixed")
                {
                    var partsWithAtt = firstMessage.Payload.Parts.Skip(1);
                    foreach (var part in partsWithAtt)
                    {
                        Attachment newAttachment = new Attachment
                        {
                            FileName = part.Filename,
                            FileSizeInMb = (double)part.Body.Size / (1024 * 1024)
                        };
                        attachments.Add(newAttachment);
                        await this.context.Attachments.AddAsync(newAttachment);
                        //this.context.SaveChanges();
                    }
                }
                //this.context.Attachments.AddRange(attachments);
                //allAttachments.AddRange(attachments);
                Email newMail = new Email
                {
                    DateReceived = date,
                    Attachments = attachments,
                    GmailIdNumber = messageId,
                    InitialRegistrationInSystemOn = DateTime.Now,
                    SetInCurrentStatusOn = DateTime.Now,
                    Sender = sender,
                    Status = status,
                    Subject = subject
                };

                //newMails.Add(newMail);

                await this.context.Emails.AddAsync(newMail);
                await this.context.SaveChangesAsync();
                result.RemoveAt(0);
                countNewMails--;
            }

        }

        public async Task DownloadBodyToMail(string messageId)
        {
            var message = this.service.Users.Messages.Get(userId, messageId).Execute();

            switch (message.Payload.MimeType)
            {
                //plain text, no attachments
                case "text/plain":
                    await ParsePlainTextMail(message);
                    break;

                //html, no attachments
                case "multipart/alternative":
                    await ParseHtmlMail(message);
                    break;

                //plain/html text, with attachments
                case "multipart/mixed":
                    await ParseMailWithAttachments(message);
                    break;

                default:
                    break;
            }
        }

        private async Task ParseMailWithAttachments(Message message)
        {
            var mail = await this.context.Emails
                .FirstOrDefaultAsync(m => m.GmailIdNumber == message.Id);

            var mailData = message.Payload.Parts[0].Parts[1].Body.Data ?? message.Payload.Parts[0].Parts[0].Body.Data;

            mail.Body = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));

            await this.context.SaveChangesAsync();
        }

        private async Task ParseHtmlMail(Message message)
        {
            var mail = await this.context.Emails
                .FirstOrDefaultAsync(m => m.GmailIdNumber == message.Id);
            var mailData = message.Payload.Parts[1].Body.Data;
            mail.Body = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));

            await this.context.SaveChangesAsync();
        }

        private async Task ParsePlainTextMail(Message message)
        {
            var mail = await this.context.Emails
                .FirstOrDefaultAsync(m => m.GmailIdNumber == message.Id);
            var mailData = message.Payload.Body.Data;
            mail.Body = Encoding.UTF8.GetString(FromBase64ForUrlString(mailData));

            await this.context.SaveChangesAsync();
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

        public IQueryable<Email> ReadAllMailsFromDb()
        {
            var allMails = this.context.Emails
                                       .Include(e => e.Attachments)
                                       .Include(e => e.Sender)
                                       .Include(e => e.Status);
            return allMails;
        }
    }
}
