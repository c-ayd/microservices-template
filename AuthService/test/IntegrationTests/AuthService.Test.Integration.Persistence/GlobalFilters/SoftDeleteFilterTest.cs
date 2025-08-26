using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.GlobalFilters
{
    [Collection(nameof(AppDbContextCollection))]
    public class SoftDeleteFilterTest
    {
        private readonly AppDbContext _appDbContext;

        public SoftDeleteFilterTest(AppDbContextFixture appDbContextFixture)
        {
            _appDbContext = appDbContextFixture.DbContext;
        }

        [Fact]
        public async Task SoftDeleteFilter_WhenEntityIsSoftDeleteableAndIsDeleted_ShouldNotAppearInResult()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            _appDbContext.Users.Remove(user);
            await _appDbContext.SaveChangesAsync();

            // Assert
            _appDbContext.UntrackEntity(user);
            var result = await _appDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.Null(result);
        }
    }
}
