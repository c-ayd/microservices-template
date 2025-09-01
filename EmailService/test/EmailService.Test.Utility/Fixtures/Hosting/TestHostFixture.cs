using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailService.Test.Utility.Fixtures.Hosting
{
    public class TestHostFixture : IAsyncLifetime
    {
        public IHost Host { get; private set; } = null!;
        public HttpClient Client { get; private set; } = null!;

        public IConfiguration Configuration { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Configuration = ConfigurationHelper.CreateConfiguration();

            Host = new HostBuilder()
                .ConfigureWebHost(hostBuilder =>
                {
                    hostBuilder.UseTestServer()
                        .UseConfiguration(Configuration)
                        .ConfigureServices((context, services) =>
                        {
                            services.AddServices(context.Configuration);
                        })
                        .Configure(appBuilder =>
                        {
                            appBuilder.Build();
                        });
                })
                .Build();

            await Host.StartAsync();
            Client = Host.GetTestClient();
        }

        public async Task DisposeAsync()
        {
            await Host.StopAsync();
            Host.Dispose();
            Client.Dispose();
        }

        public void SetDefaultOptions()
        {
            EmailHelper.SetEmailSenderResult(true);
        }
    }
}
