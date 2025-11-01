// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HuoshanAI.Proxy
{
    /// <summary>
    /// Used in ASP.NET Core WebApps to start your own HuoshanAI web api proxy.
    /// </summary>
    public class HuoshanProxy
    {
        private HuoshanClient HuoshanAIClient;
        private IAuthenticationFilter authenticationFilter;

        /// <summary>
        /// Configures the <see cref="HuoshanAIClient"/> and <see cref="IAuthenticationFilter"/> services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        public void ConfigureServices(IServiceCollection services)
            => SetupServices(services.BuildServiceProvider());

        /// <summary>
        /// Configures the <see cref="IApplicationBuilder"/> to handle requests and forward them to HuoshanAI API.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SetupServices(app.ApplicationServices);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/health", HealthEndpoint);
                endpoints.MapHuoshanAIEndpoints(HuoshanAIClient, authenticationFilter);
            });
        }

        /// <summary>
        /// Creates a new <see cref="IHost"/> that acts as a proxy web api for HuoshanAI.
        /// </summary>
        /// <typeparam name="T"><see cref="IAuthenticationFilter"/> type to use to validate your custom issued tokens.</typeparam>
        /// <param name="args">Startup args.</param>
        /// <param name="HuoshanAIClient"><see cref="HuoshanAIClient"/> with configured <see cref="HuoshanAuthentication"/> and <see cref="HuoshanSettings"/>.</param>
        public static IHost CreateDefaultHost<T>(string[] args, HuoshanClient HuoshanAIClient) where T : class, IAuthenticationFilter
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<HuoshanProxy>();
                    webBuilder.ConfigureKestrel(ConfigureKestrel);
                })
                .ConfigureLogging(logger =>
                {
                    logger.ClearProviders();
                    logger.AddConsole();
                    logger.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(HuoshanAIClient);
                    services.AddSingleton<IAuthenticationFilter, T>();
                }).Build();

        /// <summary>
        /// Creates a new <see cref="WebApplication"/> that acts as a proxy web api for HuoshanAI.
        /// </summary>
        /// <typeparam name="T"><see cref="IAuthenticationFilter"/> type to use to validate your custom issued tokens.</typeparam>
        /// <param name="args">Startup args.</param>
        /// <param name="HuoshanAIClient"><see cref="HuoshanAIClient"/> with configured <see cref="HuoshanAuthentication"/> and <see cref="HuoshanSettings"/>.</param>
        /// <param name="routePrefix"></param>
        public static WebApplication CreateWebApplication<T>(string[] args, HuoshanClient HuoshanAIClient, string routePrefix = "") where T : class, IAuthenticationFilter
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.WebHost.ConfigureKestrel(ConfigureKestrel);
            builder.Services.AddSingleton(HuoshanAIClient);
            builder.Services.AddSingleton<IAuthenticationFilter, T>();
            var app = builder.Build();
            var startup = new HuoshanProxy();
            startup.Configure(app, app.Environment);
            return app;
        }

        private static void ConfigureKestrel(KestrelServerOptions options)
        {
            options.AllowSynchronousIO = false;
            options.Limits.MinRequestBodyDataRate = null;
            options.Limits.MinResponseDataRate = null;
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
        }

        private void SetupServices(IServiceProvider serviceProvider)
        {
            HuoshanAIClient ??= serviceProvider.GetRequiredService<HuoshanClient>();
            authenticationFilter ??= serviceProvider.GetRequiredService<IAuthenticationFilter>();
        }

        private static async Task HealthEndpoint(HttpContext context)
        {
            // Respond with a 200 OK status code and a plain text message
            context.Response.StatusCode = StatusCodes.Status200OK;
            const string contentType = "text/plain";
            context.Response.ContentType = contentType;
            const string content = "OK";
            await context.Response.WriteAsync(content);
        }
    }
}
