using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.GlobalFilters
{
    [Collection(nameof(AuthDbContextCollection))]
    public class SoftDeleteFilterTest
    {
        private readonly AuthDbContext _authDbContext;

        public SoftDeleteFilterTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;
        }

        [Fact]
        public async Task SoftDeleteFilter_WhenEntityIsSoftDeleteableAndIsDeleted_ShouldNotAppearInResult()
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
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.Null(result);
        }
    }
}
