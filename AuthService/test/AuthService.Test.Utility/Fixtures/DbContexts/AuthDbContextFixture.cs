using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthService.Persistence.DbContexts;
using AuthService.Application.Settings;

namespace AuthService.Test.Utility.Fixtures.DbContexts
{
    public class AuthDbContextFixture : IAsyncLifetime
    {
        public IConfiguration Configuration { get; private set; } = null!;
        public AuthDbContext DbContext { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Configuration = ConfigurationHelper.CreateConfiguration();

            var connectionStrings = Configuration.GetSection(ConnectionStringsSettings.SettingsKey).Get<ConnectionStringsSettings>()!;
            var dbContextOptions = new DbContextOptionsBuilder<AuthDbContext>()
                .UseNpgsql(connectionStrings.Auth)
                .Options;

            DbContext = new AuthDbContext(dbContextOptions);

            if (await DbContext.Database.CanConnectAsync())
            {
                await DbContext.Database.EnsureCreatedAsync();
            }

            await DbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }
    }
}
