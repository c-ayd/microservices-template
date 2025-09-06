using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement.Enums;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.Repositories.UserManagement;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Repositories.UserManagement
{
    [Collection(nameof(AuthDbContextCollection))]
    public class TokenRepositoryTest
    {
        private readonly AuthDbContext _authDbContext;

        private readonly TokenRepository _tokenRepository;

        public TokenRepositoryTest(AuthDbContextFixture authDbContextFixture)
        {
            _authDbContext = authDbContextFixture.DbContext;

            _tokenRepository = new TokenRepository(_authDbContext);
        }

        [Fact]
        public async Task AddAsync_WhenTokenIsGiven_ShouldAddToken()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var token = new Token()
            {
                ValueHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };

            // Act
            await _tokenRepository.AddAsync(token);
            await _authDbContext.SaveChangesAsync();

            // Assert
            var tokenId = token.Id;
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(token);
            var result = await _authDbContext.Tokens
                .Where(t => t.Id.Equals(tokenId))
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTokenExists_ShouldReturnToken()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var token = new Token()
            {
                ValueHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };
            await _tokenRepository.AddAsync(token);
            await _authDbContext.SaveChangesAsync();

            var tokenId = token.Id;
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(token);

            // Act
            var result = await _tokenRepository.GetByIdAsync(tokenId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTokenDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _tokenRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByValueAndPurposeAsync_WhenTokenExists_ShouldReturnToken()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var tokenValue = StringGenerator.GenerateUsingAsciiChars(10);
            var tokenPurpose = ETokenPurpose.ResetPassword;
            var token = new Token()
            {
                ValueHashed = tokenValue,
                Purpose = tokenPurpose,
                UserId = user.Id
            };
            await _tokenRepository.AddAsync(token);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(token);

            // Act
            var result = await _tokenRepository.GetByHashedValueAndPurposeAsync(tokenValue, tokenPurpose);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByValueAndPurposeAsync_WhenTokenDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var tokenValue = StringGenerator.GenerateUsingAsciiChars(10);
            var token = new Token()
            {
                ValueHashed = tokenValue,
                Purpose = ETokenPurpose.ResetPassword,
                UserId = user.Id
            };
            await _tokenRepository.AddAsync(token);
            await _authDbContext.SaveChangesAsync();

            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(token);

            // Act
            var result = await _tokenRepository.GetByHashedValueAndPurposeAsync(tokenValue, ETokenPurpose.EmailVerification);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenTokensRelatedToUserExist_ShouldReturnAllTokens()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Tokens = new List<Token>()
            {
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10) },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10) }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _tokenRepository.GetAllByUserIdAsync(userId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenTokensRelatedToUserDoNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _tokenRepository.GetAllByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WhenUserDoesNotExist_ShouldReturnEmptyList()
        {
            // Act
            var result = await _tokenRepository.GetAllByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAndPurposeAsync_WhenTokensRelatedToUserAndPurposeExist_ShouldReturnAllRelatedTokens()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Tokens = new List<Token>()
            {
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.EmailVerification },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _tokenRepository.GetAllByUserIdAndPurposeAsync(userId, ETokenPurpose.ResetPassword);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllByUserIdAndPurposeAsync_WhenTokensRelatedToUserAndPurposeDoNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Tokens = new List<Token>()
            {
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);

            // Act
            var result = await _tokenRepository.GetAllByUserIdAndPurposeAsync(userId, ETokenPurpose.EmailVerification);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllByUserIdAndPurposeAsync_WhenUserDoesNotExist_ShouldReturnEmptyList()
        {
            // Act
            var result = await _tokenRepository.GetAllByUserIdAndPurposeAsync(Guid.NewGuid(), ETokenPurpose.EmailVerification);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Delete_WhenTokenExists_ShouldDeleteToken()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            var token = new Token()
            {
                ValueHashed = StringGenerator.GenerateUsingAsciiChars(10),
                UserId = user.Id
            };
            await _tokenRepository.AddAsync(token);
            await _authDbContext.SaveChangesAsync();

            var tokenId = token.Id;

            // Act
            _tokenRepository.Delete(token);
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            _authDbContext.UntrackEntity(token);
            var result = await _authDbContext.Tokens
                .Where(t => t.Id.Equals(tokenId))
                .FirstOrDefaultAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAll_WhenTokensExist_ShouldDeleteTokens()
        {
            // Arrange
            var user = new User();
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            user.Tokens = new List<Token>()
            {
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
            };

            await _authDbContext.SaveChangesAsync();

            var userId = user.Id;

            // Act
            _tokenRepository.DeleteAll(user.Tokens);
            await _authDbContext.SaveChangesAsync();

            // Assert
            _authDbContext.UntrackEntities(user.Tokens.ToArray());
            _authDbContext.UntrackEntity(user);
            var result = await _authDbContext.Tokens
                .Where(t => t.UserId.Equals(userId))
                .ToListAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteAllByUserIdAsync_WhenTokensRelatedToUserExist_ShouldDeleteAllTokensRelatedToUser()
        {
            // Arrange
            var user1 = new User()
            {
                Tokens = new List<Token>()
                {
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
                }
            };
            var user2 = new User()
            {
                Tokens = new List<Token>()
                {
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
                }
            };

            await _authDbContext.Users.AddAsync(user1);
            await _authDbContext.Users.AddAsync(user2);
            await _authDbContext.SaveChangesAsync();

            var userId1 = user1.Id;
            var userId2 = user2.Id;
            _authDbContext.UntrackEntities(user1.Tokens.ToArray());
            _authDbContext.UntrackEntity(user1);
            _authDbContext.UntrackEntities(user2.Tokens.ToArray());
            _authDbContext.UntrackEntity(user2);

            // Act
            var result = await _tokenRepository.DeleteAllByUserIdAsync(userId1);

            // Assert
            Assert.Equal(user1.Tokens.Count, result);

            var user1Tokens = await _authDbContext.Tokens
                .Where(l => l.UserId.Equals(userId1))
                .ToListAsync();
            Assert.Empty(user1Tokens);

            var user2Tokens = await _authDbContext.Tokens
                .Where(l => l.UserId.Equals(userId2))
                .ToListAsync();
            Assert.Equal(user2.Tokens.Count, user2Tokens.Count);
        }

        [Fact]
        public async Task DeleteAllByUserIdAsync_WhenTokensRelatedToUserDoNotExist_ShouldDeleteNothing()
        {
            // Act
            var result = await _tokenRepository.DeleteAllByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task DeleteAllByUserIdAndPurposeAsync_WhenTokensRelatedToUserAndPurposeExist_ShouldDeleteAllTokensRelatedToUserAndPurpose()
        {
            // Arrange
            var user1 = new User()
            {
                Tokens = new List<Token>()
                {
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.EmailVerification },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
                }
            };
            var user2 = new User()
            {
                Tokens = new List<Token>()
                {
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword },
                    new Token() { ValueHashed = StringGenerator.GenerateUsingAsciiChars(10), Purpose = ETokenPurpose.ResetPassword }
                }
            };

            await _authDbContext.Users.AddAsync(user1);
            await _authDbContext.Users.AddAsync(user2);
            await _authDbContext.SaveChangesAsync();

            var userId1 = user1.Id;
            var userId2 = user2.Id;
            _authDbContext.UntrackEntities(user1.Tokens.ToArray());
            _authDbContext.UntrackEntity(user1);
            _authDbContext.UntrackEntities(user2.Tokens.ToArray());
            _authDbContext.UntrackEntity(user2);

            // Act
            var result = await _tokenRepository.DeleteAllByUserIdAndPurposeAsync(userId1, ETokenPurpose.ResetPassword);

            // Assert
            Assert.Equal(user1.Tokens.Count - 1, result);

            var user1Tokens = await _authDbContext.Tokens
                .Where(l => l.UserId.Equals(userId1))
                .ToListAsync();
            Assert.Single(user1Tokens);
            Assert.Equal(ETokenPurpose.EmailVerification, user1Tokens[0].Purpose);

            var user2Tokens = await _authDbContext.Tokens
                .Where(l => l.UserId.Equals(userId2))
                .ToListAsync();
            Assert.Equal(user2.Tokens.Count, user2Tokens.Count);
        }

        [Fact]
        public async Task DeleteAllByUserIdAndPurposeAsync_WhenTokensRelatedToUserAndPurposeDoNotExist_ShouldDeleteNothing()
        {
            // Act
            var result = await _tokenRepository.DeleteAllByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }
    }
}
