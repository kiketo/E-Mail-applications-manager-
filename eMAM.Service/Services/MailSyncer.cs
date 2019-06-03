using eMAM.Service.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace eMAM.Service.Services
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
                var gmailUserService = scope.ServiceProvider.GetRequiredService<IGmailUserDataService>();


                //var gmailApiService = scope.ServiceProvider.GetRequiredService<IGmailApiService>();

                //gmailApiService.DownloadNewMailsWithoutBodyAsync().GetAwaiter().GetResult(); 

                //if ((userData.ExpiresAt - DateTime.Now).TotalMinutes < 5)
                //{
                //    var newToken = await gmailApiService.RenewAccessTokenAsync();
                //    gmailUserDataService.UpdateAsync(newToken);
                //}


                // call gmail with valid token

                // save new messages to db

                //service.RefreshTokenExpiration();

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
