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
    public class LoginRepositoryTest
    {
        private readonly AuthDbContext _authDbContext;

        private readonly LoginRepository _loginRepository;

        public LoginRepositoryTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;

            _loginRepository = new LoginRepository(_authDbContext);
        }

        [Fact]
        public async Task AddAsync_WhenLoginIsGiven_ShouldAddLogin()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var login = new Login()
            {
                RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };

            // Act
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            // Assert
            var loginId = login.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);
            var result = await _authDbContext.Logins
                .Where(l => l.Id.Equals(loginId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenLoginExists_ShouldReturnLogin()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var login = new Login()
            {
                RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            var loginId = login.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);

            // Act
            var result = await _loginRepository.GetByIdAsync(loginId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenLoginDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _loginRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAndRefreshTokenAsync_WhenUserIdAndRefreshTokenExist_ShouldReturnLogin()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var refreshToken = StringGenerator.GenerateUsingAsciiChars(10);
            var login = new Login()
            {
                RefreshTokenHashed = refreshToken,
                UserId = user.Id
            };
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);

            // Act
            var result = await _loginRepository.GetByUserIdAndHashedRefreshTokenAsync(userId, refreshToken);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByUserIdAndRefreshTokenAsync_WhenUserIdExistsButRefreshTokenDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var refreshToken = StringGenerator.GenerateUsingAsciiChars(10);
            var login = new Login()
            {
                RefreshTokenHashed = refreshToken,
                UserId = user.Id
            };
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);

            // Act
            var result = await _loginRepository.GetByUserIdAndHashedRefreshTokenAsync(userId, StringGenerator.GenerateUsingAsciiChars(10));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAndRefreshTokenAsync_WhenUserIdDoesNotExistButRefreshTokenExists_ShouldReturnNull()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var refreshToken = StringGenerator.GenerateUsingAsciiChars(10);
            var login = new Login()
            {
                RefreshTokenHashed = refreshToken,
                UserId = user.Id
            };
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);

            // Act
            var result = await _loginRepository.GetByUserIdAndHashedRefreshTokenAsync(Guid.NewGuid(), refreshToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAndRefreshTokenAsync_WhenUserIdAndRefreshTokenDoNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _loginRepository.GetByUserIdAndHashedRefreshTokenAsync(Guid.NewGuid(), StringGenerator.GenerateUsingAsciiChars(10));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenLoginsRelatedToUserExist_ShouldReturnAllLogins()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Logins = new List<Login>()
            {
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.GetAllByUserIdAsync(userId);

            // Assert
            Assert.Equal(user.Logins.Count, result.Count);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenLoginsRelatedToUserDoNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.GetAllByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenUserDoesNotExist_ShouldReturnEmptyList()
        {
            // Act
            var result = await _loginRepository.GetAllByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Delete_WhenLoginExists_ShouldDeleteLogin()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var login = new Login()
            {
                RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };
            await _loginRepository.AddAsync(login);
            await _authDbContext.SaveChangesAsync();

            var loginId = login.Id;
            
            // Act
            _loginRepository.Delete(login);
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(login);
            var result = await _authDbContext.Logins
                .Where(l => l.Id.Equals(loginId))
                .FirstOrDefaultAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAll_WhenLoginsExist_ShouldDeleteLogins()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Logins = new List<Login>()
            {
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            _loginRepository.DeleteAll(user.Logins);
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Logins
                .Where(l => l.UserId.Equals(userId))
                .ToListAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteAllByUserIdAsync_WhenLoginsRelatedToUserExist_ShouldDeleteAllLoginsRelatedToUser()
        {
            // Arrange
            var user1 = new User()
            {
                Logins = new List<Login>()
                {
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
                }
            };
            var user2 = new User()
            {
                Logins = new List<Login>()
                {
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                    new Login() { RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10) }
                }
            };

            await _authDbContext.Users.AddAsync(user1);
            await _authDbContext.Users.AddAsync(user2);
            await _authDbContext.SaveChangesAsync();

            var userId1 = user1.Id;
            var userId2 = user2.Id;
            _authDbContext.UntrackEntities(user1.Logins.ToArray());
            _authDbContext.UntrackEntity(user1);
            _authDbContext.UntrackEntities(user2.Logins.ToArray());
            _authDbContext.UntrackEntity(user2);

            // Act
            var result = await _loginRepository.DeleteAllByUserIdAsync(userId1);

            // Assert
            Assert.Equal(user1.Logins.Count, result);

            var user1Logins = await _authDbContext.Logins
                .Where(l => l.UserId.Equals(userId1))
                .ToListAsync();
            Assert.Empty(user1Logins);

            var user2Logins = await _authDbContext.Logins
                .Where(l => l.UserId.Equals(userId2))
                .ToListAsync();
            Assert.Equal(user2.Logins.Count, user2Logins.Count);
        }

        [Fact]
        public async Task DeleteAllByUserIdAsync_WhenLoginsRelatedToUserDoNotExist_ShouldDeleteNothing()
        {
            // Act
            var result = await _loginRepository.DeleteAllByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetAllActiveByUserIdAsync_WhenThereIsNoLogins_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User();

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.GetAllActiveByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllActiveByUserIdAsync_WhenThereIsNoActiveLogins_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User()
            {
                Logins = new List<Login>()
                {
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(-1)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(-1)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(-1)
                    }
                }
            };

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.GetAllActiveByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllActiveByUserIdAsync_WhenThereAreLogins_ShouldReturnOnlyActiveLogins()
        {
            // Arrange
            var user = new User()
            {
                Logins = new List<Login>()
                {
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(1)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(1)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10),
                        ExpirationDate = DateTime.UtcNow.AddDays(-1)
                    }
                }
            };

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.GetAllActiveByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenLoginDoesNotExist_ShouldReturnZero()
        {
            // Act
            var result = await _loginRepository.DeleteByIdAndUserIdAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenLoginExistsButGivenUserIdIsWrong_ShouldReturnZero()
        {
            // Arrange
            var user = new User()
            {
                Logins = new List<Login>()
                {
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    }
                }
            };

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var loginId = user.Logins[0].Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.DeleteByIdAndUserIdAsync(loginId, Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenLoginExists_ShouldReturnOneAndDeleteLogin()
        {
            // Arrange
            var user = new User()
            {
                Logins = new List<Login>()
                {
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    },
                    new Login()
                    {
                        RefreshTokenHashed = StringGenerator.GenerateUsingAsciiChars(10)
                    }
                }
            };

            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            var loginId = user.Logins[0].Id;
            _authDbContext.UntrackEntities(user.Logins.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _loginRepository.DeleteByIdAndUserIdAsync(loginId, userId);

            // Assert
            Assert.Equal(1, result);

            var login = await _authDbContext.Logins
                .Where(l => l.Id.Equals(loginId))
                .FirstOrDefaultAsync();
            Assert.Null(login);
        }
    }
}
