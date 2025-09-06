using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Interceptors
{
    [Collection(nameof(AuthDbContextCollection))]
    public class SoftDeleteInterceptorTest
    {
        private readonly AuthDbContext _authDbContext;

        public SoftDeleteInterceptorTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;
        }

        [Fact]
        public async Task SoftDeleteInterceptor_WhenEntityIsSoftDeleteableAndIsDeleted_ShouldNotDeleteEntityAndModifySoftDeleteProperties()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            _authDbContext.Users.Remove(user);
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Users
                .IgnoreQueryFilters()
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.True(result.IsDeleted, "The entity is not marked as deleted.");
            Assert.NotNull(result.DeletedDate);
            Assert.True(result.DeletedDate >= startTime, "The deleted date is wrong.");
        }
    }
}
