using GenericServices.Configuration;
using GenericServices.Setup;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums;
using NewAlbums.Artists;
using NewAlbums.Emails;
using NewAlbums.Emails.Templates;
using NewAlbums.EntityFrameworkCore;
using NewAlbums.Paths;
using NewAlbums.Spotify;
using NewAlbums.Subscribers;
using NewAlbums.Subscriptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NewAlbums.Notifications
{
    public class Program
    {
        /// <summary>
        /// See appsettings.json for where to configure various settings required by this console app 
        /// (user-secrets in dev, Azure app service Connection Strings and Application Settings in production)
        /// </summary>
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            IConfiguration configuration = null;

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

                configuration = configApp.Build();
            });

            builder.ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorage();
                });

            builder.ConfigureLogging((context, b) =>
                {
                    b.AddConsole();

                    if (context.HostingEnvironment.EnvironmentName.ToLower() == "development")
                    {
                        b.AddLog4Net();
                    }
                });

            builder.ConfigureServices(serviceCollection =>
            {
                //Need to do this or later when IConfiguration is injected, it'll be missing our user secrets configured for dev above
                serviceCollection.AddSingleton<IConfiguration>(configuration);

                serviceCollection.AddDbContext<NewAlbumsDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("Default")));

                serviceCollection.GenericServicesSimpleSetup<NewAlbumsDbContext>(
                    new GenericServicesConfig
                    {
                        NoErrorOnReadSingleNull = true
                    },
                    Assembly.GetAssembly(typeof(BaseAppService))
                );

                //NewsAlbums.Application services
                serviceCollection.AddTransient<ISpotifyAppService, SpotifyAppService>();
                serviceCollection.AddTransient<IArtistAppService, ArtistAppService>();
                serviceCollection.AddTransient<IAlbumAppService, AlbumAppService>();
                serviceCollection.AddTransient<ISubscriberAppService, SubscriberAppService>();
                serviceCollection.AddTransient<ISubscriptionAppService, SubscriptionAppService>();

                //NewAlbums.Core managers
                serviceCollection.AddTransient<EmailManager>();
                serviceCollection.AddTransient<TemplateManager>();
                serviceCollection.AddTransient<IPathProvider, PathProvider>();

            });

            //Set the App_Data directory so we can retrieve it later. https://stackoverflow.com/a/48357218
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(currentDirectory, "App_Data"));

            var host = builder.Build();
            using (host)
            {
                await host.StartAsync();

                var jobHost = host.Services.GetService(typeof(IJobHost)) as JobHost;
                await jobHost.CallAsync(typeof(Functions).GetMethod("ProcessNewSpotifyAlbums"));

                await host.StopAsync();
            }
        }
    }
}
