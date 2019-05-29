using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eMAM.Service.Services
{
    public class MailSyncer : IHostedService
    {
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public MailSyncer(ILogger<MailSyncer> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is starting.");

            this.timer = new Timer(GetNewEmailsFromGmail, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void GetNewEmailsFromGmail(object state)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                // check access_token expiration

                // renew with refresh token if necessary

                // call gmail with valid token

                // save new messages to db

                //var service = scope.ServiceProvider.GetRequiredService<ScopedService>();

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
