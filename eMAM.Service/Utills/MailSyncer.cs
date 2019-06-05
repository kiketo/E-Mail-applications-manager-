using eMAM.Data.Models;
using eMAM.Service.DbServices;
using eMAM.Service.DbServices.Contracts;
using eMAM.Service.GmailServices.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eMAM.Service.Utills
{
    public class MailSyncer : IHostedService
    {
        private readonly ILogger<MailSyncer> logger;
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public MailSyncer(ILogger<MailSyncer> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is starting.");

            this.timer = new Timer(GetNewEmailsFromGmail, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(300));

            return Task.CompletedTask;
        }

        private void GetNewEmailsFromGmail(object state)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var gmailUserDataService = scope.ServiceProvider.GetRequiredService<IGmailUserDataService>();
                var gmailApiService = scope.ServiceProvider.GetRequiredService<IGmailApiService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var statusService = scope.ServiceProvider.GetRequiredService<IStatusService>();
                var senderService = scope.ServiceProvider.GetRequiredService<ISenderService>();
                var attachmentService = scope.ServiceProvider.GetRequiredService<IAttachmentService>();

                var userData = gmailUserDataService.GetAsync().GetAwaiter().GetResult();

                if (!gmailUserDataService.IsAccessTokenValidAsync().GetAwaiter().GetResult())
                {
                    var newAccessDTO=gmailApiService.RenewAccessTokenAsync(userData.RefreshToken).GetAwaiter().GetResult();

                    gmailUserDataService.UpdateAsync(newAccessDTO).GetAwaiter().GetResult();
                    userData = gmailUserDataService.GetAsync().GetAwaiter().GetResult();
                }

                var gmailMessagesList = gmailApiService.DownloadMailsListAsync(userData.AccessToken).GetAwaiter().GetResult();
                int countNewMails = gmailMessagesList.Count - emailService.GetDbEmailsCountAsync().GetAwaiter().GetResult();

                var status = statusService.GetInitialStatusAsync().GetAwaiter().GetResult();


                while (true)
                {
                    if (countNewMails == 0)
                    {
                        break;
                    }
                    var messageId = gmailMessagesList.Messages.First().Id;
                    var messageAlreadyInDb = emailService.IsEmailInDbAsync(messageId).GetAwaiter().GetResult();

                    if (messageAlreadyInDb)
                    {
                        gmailMessagesList.Messages.RemoveAt(0);
                        continue;
                    }

                    var firstMessage = gmailApiService.DownloadMail(messageId, userData.AccessToken).GetAwaiter().GetResult();

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

                    var sender = senderService.GetSenderAsync(senderMail, senderName).GetAwaiter().GetResult();
                    if (sender == null)
                    {
                        sender = senderService.AddSenderAsync(senderMail, senderName).GetAwaiter().GetResult();
                        sender = new Sender { SenderEmail = senderMail, SenderName = senderName };
                    }

                    List<Attachment> attachments = new List<Attachment>();
                    //if (firstMessage.Payload.MimeType == "multipart/mixed")
                    //{
                    //    var partsWithAtt = firstMessage.Payload.Parts.Skip(1);
                    //    foreach (var part in partsWithAtt)
                    //    {
                    //        Attachment newAttachment = attachmentService
                    //                                .AddAttachmentAsync(part.FileName, part.Body.SizeInBytes)
                    //                                .GetAwaiter().GetResult();
                    //    }
                    //}
                    emailService.AddEmailAsync(date, attachments, messageId, sender,status,subject).GetAwaiter().GetResult();

                    gmailMessagesList.Messages.RemoveAt(0);
                    countNewMails--;
                }


                //gmailApiService.DownloadNewMailsJson(userData).GetAwaiter().GetResult();

                //this.logger.LogInformation("Scoped service id: " + service.Id);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is stopping.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }


    }
}
