using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Interceptors
{
    [Collection(nameof(AuthDbContextCollection))]
    public class CreatedDateInterceptorTest
    {
        private readonly AuthDbContext _authDbContext;

        public CreatedDateInterceptorTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;
        }

        [Fact]
        public async Task CreatedDateInterceptor_WhenNewEntityIsAddedToDb_ShouldAlsoAddCreatedDateToEntity()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            // Act
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Assert
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.True(result.CreatedDate >= startTime, "The created date is wrong.");
        }
    }
}
