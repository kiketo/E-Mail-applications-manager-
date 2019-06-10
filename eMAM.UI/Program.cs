using eMAM.Service.Utills;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace eMAM.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            DataSeed.SeedDatabaseWithSuperAdminAsync(host).Wait();
            DataSeed.SeedDatabaseWithStatus(host).Wait();
            DataSeed.SeedToken(host).Wait();
            host.Run();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .Build();
    }
}
