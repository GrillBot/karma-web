using Discord.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RubberWeb.Services;

namespace RubberWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Default");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            services
                .AddLogging(config => config.SetMinimumLevel(LogLevel.Debug).AddConsole().AddDebug())
                .AddMemoryCache()
                .AddControllersWithViews();

            services
                .AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

            var discordConfig = new DiscordRestConfig()
            {
                LogLevel = Discord.LogSeverity.Verbose
            };

            services.AddSingleton<UserService>()
                .AddSingleton(new DiscordRestClient(discordConfig));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app
                .UseStaticFiles()
                .UseMiddleware<DelayMiddleware>();

            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            app
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
                })
                .UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                        spa.UseAngularCliServer(npmScript: "start");
                });
        }
    }
}
