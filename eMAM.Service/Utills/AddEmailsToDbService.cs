using eMAM.Data.Models;
using eMAM.Service.DbServices;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.GmailServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.Utills
{
    public class AddEmailsToDbService : IAddEmailsToDbService
    {
        private IGmailUserDataService gmailUserDataService;
        private IGmailApiService gmailApiService;
        private IEmailService emailService;
        private IStatusService statusService;
        private ISenderService senderService;
        private IAttachmentService attachmentService;

        public AddEmailsToDbService(IGmailUserDataService gmailUserDataService, IGmailApiService gmailApiService, IEmailService emailService, IStatusService statusService, ISenderService senderService, IAttachmentService attachmentService)
        {
            this.gmailUserDataService = gmailUserDataService ?? throw new ArgumentNullException(nameof(gmailUserDataService));
            this.gmailApiService = gmailApiService ?? throw new ArgumentNullException(nameof(gmailApiService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            this.senderService = senderService ?? throw new ArgumentNullException(nameof(senderService));
            this.attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        }

        public async Task Execute()
        {
            var userData = await this.gmailUserDataService.GetAsync();

            if (!await this.gmailUserDataService.IsAccessTokenValidAsync())
            {
                var newAccessDTO = await gmailApiService.RenewAccessTokenAsync(userData.RefreshToken);

                await gmailUserDataService.UpdateAsync(newAccessDTO);
               // userData = await gmailUserDataService.GetAsync();
            }

            var gmailMessagesList = await gmailApiService.DownloadMailsListAsync(userData.AccessToken);
            int countNewMails = gmailMessagesList.Count - await emailService.GetDbEmailsCountAsync();

            var status = await statusService.GetInitialStatusAsync();


            while (true)
            {
                if (countNewMails == 0)
                {
                    break;
                }
                var messageId = gmailMessagesList.Messages.First().Id;
                var messageAlreadyInDb = await emailService.IsEmailInDbAsync(messageId);

                if (messageAlreadyInDb)
                {
                    gmailMessagesList.Messages.RemoveAt(0);
                    continue;
                }

                var firstMessage = await gmailApiService.DownloadMail(messageId, userData.AccessToken);

                var headers = firstMessage.Payload.Headers;

                //Gmail epoch time in ms
                var gmail_date = (long)firstMessage.InternalDate;

                //Get DateTime of epoch ms
                var to_date = DateTimeOffset.FromUnixTimeMilliseconds(gmail_date).DateTime;

                //This is your timezone offset GMT +3
                var offset = 3;

                var date = to_date - new TimeSpan(offset * -1, 0, 0);

                var subject = headers.FirstOrDefault(s => s.Name == "Subject").Value;
                var senderFull = headers.FirstOrDefault(s => s.Name == "From").Value;
                var senderMail = senderFull.Substring(senderFull.IndexOf('<') + 1).Replace('>', ' ').TrimEnd();
                var index = senderFull.IndexOf('<');
                var senderName = senderFull.Substring(0, index - 1);

                var sender = await senderService.GetSenderAsync(senderMail, senderName);
                if (sender == null)
                {
                    sender = await this.senderService.AddSenderAsync(senderMail, senderName);
                    //sender = new Sender { SenderEmail = senderMail, SenderName = senderName };
                }

                List<Attachment> attachments = new List<Attachment>();
                if (firstMessage.Payload.MimeType == "multipart/mixed")
                {
                    var partsWithAtt = firstMessage.Payload.Parts.Skip(1);
                    foreach (var part in partsWithAtt)
                    {
                        attachments.Add(new Attachment
                        {
                            FileName = part.FileName,
                            FileSizeInMb = (double)part.Body.SizeInBytes / (1024 * 1024)
                        });
                    }
                }
                await emailService.AddEmailAsync(date, attachments, messageId, sender, status, subject);

                gmailMessagesList.Messages.RemoveAt(0);

                countNewMails--;
            }
        }
    }
}
