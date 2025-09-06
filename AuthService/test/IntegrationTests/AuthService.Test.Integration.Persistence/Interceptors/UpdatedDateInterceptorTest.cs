using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Interceptors
{
    [Collection(nameof(AuthDbContextCollection))]
    public class UpdatedDateInterceptorTest
    {
        private readonly AuthDbContext _authDbContext;

        public UpdatedDateInterceptorTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;
        }

        [Fact]
        public async Task UpdatedDateInterceptor_WhenEntityIsUpdated_ShouldAlsoUpdateUpdatedDate()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            user.Email = EmailGenerator.Generate();
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.NotNull(result.UpdatedDate);
            Assert.True(result.UpdatedDate >= startTime, "The updated date is wrong.");
        }
    }
}
