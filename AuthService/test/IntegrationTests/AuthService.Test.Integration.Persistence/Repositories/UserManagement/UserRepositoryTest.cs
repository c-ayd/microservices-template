using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.Repositories.UserManagement;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Repositories.UserManagement
{
    [Collection(nameof(AuthDbContextCollection))]
    public class UserRepositoryTest
    {
        private readonly AuthDbContext _authDbContext;
        private readonly UserRepository _userRepository;

        public UserRepositoryTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;

            _userRepository = new UserRepository(_authDbContext);
        }

        [Fact]
        public async Task AddAsync_WhenNewUserIsGiven_ShouldAddUser()
        {
            // Arrange
            var user = new User();

            // Act
            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            // Arrange
            var userId = user.Id;
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User();

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetByEmailAsync(EmailGenerator.Generate());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdWithSecurityStateAsync_WhenUserExists_ShouldReturnUserWithSecurityState()
        {
            // Arrange
            var user = new User()
            {
                SecurityState = new SecurityState()
                {
                    FailedAttempts = 2
                }
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetByIdWithSecurityStateAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SecurityState);
            Assert.Equal(2, result.SecurityState.FailedAttempts);
        }

        [Fact]
        public async Task GetByIdWithSecurityStateAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetByIdWithSecurityStateAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailWithSecurityStateAsync_WhenUserExists_ShouldReturnUserWithSecurityState()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    FailedAttempts = 2
                }
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetByEmailWithSecurityStateAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SecurityState);
            Assert.Equal(2, result.SecurityState.FailedAttempts);
        }

        [Fact]
        public async Task GetByEmailWithSecurityStateAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetByEmailWithSecurityStateAsync(EmailGenerator.Generate());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetIdByEmailAsync_WhenUserExists_ShouldReturnUserId()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetIdByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task GetIdByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetIdByEmailAsync(EmailGenerator.Generate());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEmailByIdAsync_WhenUserExists_ShouldReturnEmail()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetEmailByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email.ToLower(), result);
        }

        [Fact]
        public async Task GetEmailByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetEmailByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSecurityStateByIdAsync_WhenUserExists_ShouldReturnSecurityState()
        {
            // Arrange
            var user = new User()
            {
                SecurityState = new SecurityState()
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetSecurityStateByIdAsync(userId);

            // Arrange
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSecurityStateByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetSecurityStateByIdAsync(Guid.NewGuid());

            // Arrange
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSecurityStateByEmailAsync_WhenUserExists_ShouldReturnUserIdAndSecurityState()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetSecurityStateByEmailAsync(email);

            // Arrange
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetSecurityStateByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetSecurityStateByEmailAsync(EmailGenerator.Generate());

            // Arrange
            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_WhenUserIsGiven_ShouldSoftDeleteUser()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var user = new User();

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            _userRepository.Delete(user);
            await _authDbContext.SaveChangesAsync();

            // Arrange
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Users
                .IgnoreQueryFilters()
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.True(result.IsDeleted, "The user is not marked as deleted.");
            Assert.True(result.DeletedDate >= startTime, "The deleted date is wrong.");
        }

        [Fact]
        public async Task GetRolesByIdAsync_WhenUserHasRoles_ShouldReturnAllRoles()
        {
            // Arrange
            var roles = new List<Role>()
            {
                new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) },
                new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) }
            };
            
            var user = new User();
            user.Roles = roles;

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetRolesByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roles.Count, result.Count);
        }

        [Fact]
        public async Task GetRolesByIdAsync_WhenUserHasNoRole_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User();
            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetRolesByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRolesByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetRolesByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRolesByEmailAsync_WhenUserHasRoles_ShouldReturnAllRoles()
        {
            // Arrange
            var roles = new List<Role>()
            {
                new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) },
                new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) }
            };

            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };
            user.Roles = roles;

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetRolesByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roles.Count, result.Count);
        }

        [Fact]
        public async Task GetRolesByEmailAsync_WhenUserHasNoRole_ShouldReturnEmptyList()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetRolesByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRolesByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetRolesByEmailAsync(EmailGenerator.Generate());

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        public async Task GetAllAsync_WhenPageOrPageSizeIsNegative_ShouldThrowException(int page, int pageSize)
        {
            // Act
            var result = await Record.ExceptionAsync(async () =>
            {
                await _userRepository.GetAllAsync(page, pageSize, 5);
            });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenPaginationIsOutOfRange_ShouldReturnEmptyListAndZero()
        {
            // Arrange
            for (int i = 0; i < 20; ++i)
            {
                await _userRepository.AddAsync(new User());
            }

            await _authDbContext.SaveChangesAsync();

            int pageSize = 15;

            // Act
            var result = await _userRepository.GetAllAsync(int.MaxValue, pageSize, 5);

            // Assert
            var (users, numberOfNextPages) = result;
            Assert.Empty(users);
            Assert.Equal(0, numberOfNextPages);
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldReturnUsersAccordingToPaginationAndReturnNumberOfNextPages()
        {
            // Arrange
            for (int i = 0; i < 20; ++i)
            {
                await _userRepository.AddAsync(new User());
            }

            await _authDbContext.SaveChangesAsync();

            int page = 1;
            int pageSize = 15;

            // Act
            var result = await _userRepository.GetAllAsync(page, pageSize, 5);

            // Assert
            var (users, numberOfNextPages) = result;
            Assert.NotEmpty(users);
            Assert.Equal(pageSize, users.Count);
            Assert.True(numberOfNextPages > 0, "The number of next pages is zero.");
        }

        [Fact]
        public async Task GetWithFullContextById_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetWithFullContextByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWithFullContextById_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User()
            {
                SecurityState = new SecurityState(),
                Roles = new List<Role>()
                {
                    new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) }
                },
                Logins = new List<Login>()
                {
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
                }
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Roles.ToArray());
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user.SecurityState);
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetWithFullContextByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SecurityState);
            Assert.NotEmpty(result.Roles);
            Assert.NotEmpty(result.Logins);
        }

        [Fact]
        public async Task GetWithFullContextByEmail_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _userRepository.GetWithFullContextByEmailAsync(EmailGenerator.Generate());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWithFullContextByEmail_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState(),
                Roles = new List<Role>()
                {
                    new Role() { Name = StringGenerator.GenerateUsingAsciiChars(10) }
                },
                Logins = new List<Login>()
                {
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
                }
            };

            await _userRepository.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntities(user.Roles.ToArray());
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user.SecurityState);
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _userRepository.GetWithFullContextByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SecurityState);
            Assert.NotEmpty(result.Roles);
            Assert.NotEmpty(result.Logins);
        }
    }
}
