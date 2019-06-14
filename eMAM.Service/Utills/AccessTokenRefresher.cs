using eMAM.Service.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace eMAM.Service.Utills
{
    public class AccessTokenRefresher : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public AccessTokenRefresher(IServiceProvider serviceProvider, Timer timer)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.timer = timer ?? throw new ArgumentNullException(nameof(timer));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.timer = new Timer(RefreshAccessToken, null, TimeSpan.Zero,
               TimeSpan.FromSeconds(3000));

            return Task.CompletedTask;
        }

        private void RefreshAccessToken(object state)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var gmailUserDataService = scope.ServiceProvider.GetRequiredService<IGmailUserDataService>();
                var gmailApiService = scope.ServiceProvider.GetRequiredService<IGmailApiService>();

                if (!gmailUserDataService.IsAccessTokenValidAsync().GetAwaiter().GetResult())
                {
                    var userData = gmailUserDataService.GetAsync().GetAwaiter().GetResult();
                    var newAccessDTO = gmailApiService.RenewAccessTokenAsync(userData.RefreshToken).GetAwaiter().GetResult();

                    gmailUserDataService.UpdateAsync(newAccessDTO).GetAwaiter().GetResult();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
