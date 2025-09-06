using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthService.Application.Policies;
using AuthService.Persistence.Settings;
using AuthService.Test.Integration.Api.Collections;
using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Api
{
    [Collection(nameof(TestHostCollection))]
    public class SeedDataTest
    {
        private readonly TestHostFixture _testHostFixture;

        public SeedDataTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
        }

        [Fact]
        public async Task AuthDbContext_WhenApplicationStarts_ShouldSeedData()
        {
            // Arrange
            var seedDataSettings = _testHostFixture.Configuration.GetSection(SeedDataAuthDbContextSettings.SettingsKey).Get<SeedDataAuthDbContextSettings>()!;

            var adminEmails = seedDataSettings.AdminEmails
                .OrderBy(e => e)
                .ToList();

            // Assert
            var adminRole = await _testHostFixture.AuthDbContext.Roles
                .Where(r => r.Name == AdminPolicy.RoleName)
                .FirstOrDefaultAsync();
            Assert.NotNull(adminRole);

            var emails = await _testHostFixture.AuthDbContext.Users
                .Where(u => u.Email != null && adminEmails.Contains(u.Email))
                .Select(u => u.Email)
                .OrderBy(e => e)
                .ToListAsync();
            Assert.NotEmpty(emails);
            Assert.Equal(adminEmails, emails!);
        }
    }
}
