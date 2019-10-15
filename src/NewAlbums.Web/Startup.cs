using GenericServices.Configuration;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NewAlbums.Albums;
using NewAlbums.Artists;
using NewAlbums.Emails;
using NewAlbums.Emails.Templates;
using NewAlbums.EntityFrameworkCore;
using NewAlbums.Logging;
using NewAlbums.Paths;
using NewAlbums.Spotify;
using NewAlbums.Subscribers;
using NewAlbums.Subscriptions;
using NewAlbums.Web.Filters;
using NewAlbums.Web.Responses.Common;
using NewAlbums.Web.Rules;
using Newtonsoft.Json;
using NWebsec.Core.Common.Middleware.Options;
using System;
using System.IO;
using System.Reflection;

namespace NewAlbums.Web
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
            services.AddDbContext<NewAlbumsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.GenericServicesSimpleSetup<NewAlbumsDbContext>(
                new GenericServicesConfig
                {
                    NoErrorOnReadSingleNull = true
                }, 
                Assembly.GetAssembly(typeof(BaseAppService))
            );

            //NewsAlbums.Application services
            services.AddTransient<ISpotifyAppService, SpotifyAppService>();
            services.AddTransient<IArtistAppService, ArtistAppService>();
            services.AddTransient<IAlbumAppService, AlbumAppService>();
            services.AddTransient<ISubscriberAppService, SubscriberAppService>();
            services.AddTransient<ISubscriptionAppService, SubscriptionAppService>();

            //NewAlbums.Core managers
            services.AddTransient<EmailManager>();
            services.AddTransient<TemplateManager>();
            services.AddTransient<IPathProvider, PathProvider>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<RequestValidationAttribute>();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            NewAlbumsLogging.ConfigureLogger(loggerFactory);
            //Set the injected loggerFactory (which is used by ASP.NET logging messages) as the singleton instance to use everywhere
            NewAlbumsLogging.LoggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();

                var options = new RewriteOptions()
                    .AddRedirectToHttps(301)
                    .Add(new NonWwwRule());

                app.UseRewriter(options);
            }

            //NWebSec extensions
            //Other headers, from: https://damienbod.com/2018/02/08/adding-http-headers-to-improve-security-in-an-asp-net-mvc-core-application/
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.OriginWhenCrossOrigin());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());

            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .StyleSources(s => s.Self())
                .StyleSources(s => s.CustomSources("https://fonts.googleapis.com"))
                .StyleSources(s => s.UnsafeInline())
                .FontSources(s => s.Self())
                .FontSources(s => s.CustomSources("https://fonts.gstatic.com"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self())
                .ImageSources(s => s.CustomSources("https://i.scdn.co", "https://www.google-analytics.com"))
                //.ImageSources(s => s.CustomSources("data:"))
                .ScriptSources(s => s.Self())
                //Needed for webpackHotDevClient in development, and when compiled for production. 
                //TODO implement nonces instead
                .ScriptSources(s => s.UnsafeInline())
                .ScriptSources(s => s.CustomSources("https://www.google-analytics.com", "https://storage.googleapis.com"))
            );

            app.UseHttpsRedirection();

            //10 mins in dev, 365 days in production
            var maxAge = env.IsDevelopment() ? TimeSpan.FromMinutes(10) : TimeSpan.FromDays(365);

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();

                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = maxAge
                    };
                }
            });

            app.UseSpaStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();

                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = maxAge
                    };
                }
            });

            //From: https://code-maze.com/aspnetcore-webapi-best-practices/
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        var apiResponse = new ApiResponse(500, ex.Message);
                        string json = JsonConvert.SerializeObject(apiResponse);

                        await context.Response.WriteAsync(json);
                    }
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            //Set the App_Data directory so we can retrieve it later. https://stackoverflow.com/a/48357218
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(env.ContentRootPath, "App_Data"));
        }
    }
}
