using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Data.Utills;
using eMAM.Service.Utills;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace eMAM.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //registers all services from the service layer
            var serviceAssembly = Assembly.Load("eMAM.Service");
            services.Scan(scan => scan.FromAssemblies(serviceAssembly)
                 .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            //registers all mappers
            services.Scan(scan => scan.FromCallingAssembly()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Mapper")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            //services.AddDataProtection()
            //    .PersistKeysToAzureBlobStorage(new Uri("<blobUriWithSasToken>"))
            //    .ProtectKeysWithAzureKeyVault("<keyIdentifier>", "<clientId>", "<clientSecret>");

            //register MSSQL server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer($"Server={Constants.serverName};Database=eMAM;Trusted_Connection=True;"));

            //register PostGreSQL server
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseNpgsql($"Server=localhost;Port=5432;Database=eMAM;UserId=postgres;Password={Constants.posgrePassword};"));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //TODO
            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    options.AreaViewLocationFormats.Clear();
            //    options.AreaViewLocationFormats.Add("/Areas/SuperAdmin/Views");
            //    options.AreaViewLocationFormats.Add("/Views/Shared");
            //});

            //// Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //Add Kendo UI service to the service container
            services.AddKendo();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHostedService<MailSyncer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Admin}/{action=Dashboard}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
