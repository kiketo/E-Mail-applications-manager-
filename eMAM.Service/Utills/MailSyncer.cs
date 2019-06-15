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
                TimeSpan.FromSeconds(60000));//TODO to make in 60sec

            return Task.CompletedTask;
        }

        private void GetNewEmailsFromGmail(object state)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var addEmailsToDbService = scope.ServiceProvider.GetRequiredService<IAddEmailsToDbService>();

                addEmailsToDbService.Execute().GetAwaiter().GetResult();
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
