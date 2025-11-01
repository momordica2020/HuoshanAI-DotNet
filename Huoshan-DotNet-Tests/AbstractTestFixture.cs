// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;

namespace HuoshanAI.Tests
{
    internal abstract class AbstractTestFixture
    {
        protected class TestProxyFactory : WebApplicationFactory<Proxy.Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Development");
                base.ConfigureWebHost(builder);
            }
        }

        internal const string TestUserToken = "sess-aAbBcCdDeE123456789";

        protected readonly HttpClient HttpClient;

        protected readonly HuoshanClient HuoshanAIClient;

        protected AbstractTestFixture()
        {
            var webApplicationFactory = new TestProxyFactory();
            HttpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = GetBaseAddressFromLaunchSettings()
            });
            var settings = new HuoshanSettings(domain: HttpClient.BaseAddress?.Authority);
            var auth = new HuoshanAuthentication(TestUserToken);

            HuoshanAIClient = new HuoshanClient(auth, settings, HttpClient)
            {
                EnableDebug = true,
            };
        }

        private static Uri GetBaseAddressFromLaunchSettings()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettings = Path.Combine(projectDir, "Properties", "launchSettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonFile(launchSettings, optional: false)
                .Build();
            var applicationUrl = config["profiles:HuoshanAI_DotNet_Tests_Proxy:applicationUrl"];
            if (string.IsNullOrEmpty(applicationUrl))
            {
                throw new InvalidOperationException("Base address not found in launchSettings.json");
            }
            var hosts = applicationUrl.Split(";");
            return new Uri(hosts[0]);
        }
    }
}
