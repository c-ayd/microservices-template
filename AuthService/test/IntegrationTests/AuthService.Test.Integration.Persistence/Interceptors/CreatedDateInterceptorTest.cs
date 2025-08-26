using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Interceptors
{
    [Collection(nameof(AppDbContextCollection))]
    public class CreatedDateInterceptorTest
    {
        private readonly AppDbContext _appDbContext;

        public CreatedDateInterceptorTest(AppDbContextFixture appDbContextFixture)
        {
            _appDbContext = appDbContextFixture.DbContext;
        }

        [Fact]
        public async Task CreatedDateInterceptor_WhenNewEntityIsAddedToDb_ShouldAlsoAddCreatedDateToEntity()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            // Act
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Assert
            _appDbContext.UntrackEntity(user);
            var result = await _appDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.True(result.CreatedDate >= startTime, "The created date is wrong.");
        }
    }
}
