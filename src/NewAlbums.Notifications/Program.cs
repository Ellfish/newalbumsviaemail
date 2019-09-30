﻿using GenericServices.Configuration;
using GenericServices.Setup;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewAlbums.Artists;
using NewAlbums.Emails;
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
        /// Please set the following connection strings for this WebJob to run 
        /// (use user-secrets in development, App Service connection strings in Azure production)
        /// AzureWebJobsStorage
        /// </summary>
        /// <param name="args"></param>
        static async Task Main(string[] args)
        {
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

            builder.ConfigureServices(serviceCollection =>
            {
                /*
                serviceCollection.GenericServicesSimpleSetup<NewAlbumsDbContext>(
                    new GenericServicesConfig
                    {
                        NoErrorOnReadSingleNull = true
                    },
                    Assembly.GetAssembly(typeof(BaseAppService))
                );
                */

                //NewsAlbums.Application services
                serviceCollection.AddTransient<ISpotifyAppService, SpotifyAppService>();
                serviceCollection.AddTransient<IArtistAppService, ArtistAppService>();
                serviceCollection.AddTransient<ISubscriberAppService, SubscriberAppService>();
                serviceCollection.AddTransient<ISubscriptionAppService, SubscriptionAppService>();

                //NewAlbums.Core managers
                serviceCollection.AddTransient<EmailManager>();
                serviceCollection.AddTransient<IPathProvider, PathProvider>();

            });

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