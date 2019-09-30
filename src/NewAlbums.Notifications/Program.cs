using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewAlbums.Artists;
using NewAlbums.Spotify;
using NewAlbums.Subscriptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NewAlbums.Notifications
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var builder = new HostBuilder();

            builder.ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("hostsettings.json", optional: true);

                //Need to manually read ASPNETCORE_ENVIRONMENT or else it'll default to Production
                //See: https://github.com/aspnet/AspNetCore/issues/4150
                configHost.AddInMemoryCollection(new[] { new KeyValuePair<string, string>(
                    HostDefaults.EnvironmentKey, System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) });
                configHost.AddEnvironmentVariables();
            });

            builder.ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: true);
                configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                configApp.AddEnvironmentVariables();

                if (hostContext.HostingEnvironment.EnvironmentName.ToLower() == "development")
                {
                    configApp.AddUserSecrets<Program>();
                }
            });

            builder.ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorage();
                });

            builder.ConfigureLogging((context, b) =>
                {
                    b.AddConsole();
                });

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //NewsAlbums.Application services
            services.AddTransient<ISpotifyAppService, SpotifyAppService>();
            services.AddTransient<IArtistAppService, ArtistAppService>();
            services.AddTransient<ISubscriptionAppService, SubscriptionAppService>();                
        }
    }
}
