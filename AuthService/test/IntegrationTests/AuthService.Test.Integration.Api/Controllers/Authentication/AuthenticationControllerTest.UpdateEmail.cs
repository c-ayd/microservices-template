using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using AuthService.Application.Features.Commands.Authentication.UpdateEmail;
using AuthService.Application.Settings;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement.Enums;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.TestValues;

namespace AuthService.Test.Integration.Api.Controllers.Authentication
{
    public partial class AuthenticationControllerTest
    {
        public const string _updateEmailEndpoint = "/auth/update-email";

        [Fact]
        public async Task UpdateEmail_WhenNotLoggedIn_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(TestValues.GetInvalidEmails), MemberType = typeof(TestValues))]
        public async Task UpdateEmail_WhenNewEmailAddressIsInvalid_ShouldReturnBadRequest(string? email)
        {
            // Arrange
            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = email,
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(TestValues.GetInvalidPassword), MemberType = typeof(TestValues))]
        public async Task UpdateEmail_WhenPasswordIsInvalid_ShouldReturnBadRequest(string? password)
        {
            // Arrange
            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = password
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateEmail_WhenUserDoesNotExist_ShouldReturnInternalServerError()
        {
            // Arrange
            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task UpdateEmail_WhenPasswordIsWrong_ShouldReturnBadRequestAndIncreaseFailedAttemps()
        {
            // Arrange
            var user = new User()
            {
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(StringGenerator.GenerateUsingAsciiChars(15))
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntity(user.SecurityState);
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var securityState = await _testHostFixture.AuthDbContext.SecurityStates
                .Where(ss => ss.UserId.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(securityState);
            Assert.Equal(1, securityState.FailedAttempts);
        }

        [Fact]
        public async Task UpdateEmail_WhenPasswordIsWrongAndFailedAttempsReachesFirstLockCount_ShouldReturnLockedAndLockAccountAccordingly()
        {
            // Arrange
            var accountLockSettings = _testHostFixture.Configuration.GetSection(AccountLockSettings.SettingsKey).Get<AccountLockSettings>()!;

            var user = new User()
            {
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(StringGenerator.GenerateUsingAsciiChars(15)),
                    FailedAttempts = accountLockSettings.FailedAttemptsForFirstLock - 1,
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntity(user.SecurityState);
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.Locked, result.StatusCode);

            var securityState = await _testHostFixture.AuthDbContext.SecurityStates
                .Where(ss => ss.UserId.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(securityState);
            Assert.Equal(accountLockSettings.FailedAttemptsForFirstLock, securityState.FailedAttempts);
            Assert.True(securityState.IsLocked, "The account is not marked as locked.");
            Assert.NotNull(securityState.UnlockDate);

            var totalMinutes = (securityState.UnlockDate.Value - DateTime.UtcNow).TotalMinutes;
            Assert.True(totalMinutes >= accountLockSettings.FirstLockTimeInMinutes - 1 && totalMinutes <= accountLockSettings.FirstLockTimeInMinutes,
                $"The unlock date is not in range. Total minutes: {totalMinutes}");
        }

        [Fact]
        public async Task UpdateEmail_WhenPasswordIsWrongAndFailedAttempsReachesSecondLockCount_ShouldReturnLockedAndLockAccountAccordingly()
        {
            // Arrange
            var accountLockSettings = _testHostFixture.Configuration.GetSection(AccountLockSettings.SettingsKey).Get<AccountLockSettings>()!;

            var user = new User()
            {
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(StringGenerator.GenerateUsingAsciiChars(15)),
                    FailedAttempts = accountLockSettings.FailedAttemptsForSecondLock - 1,
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false)
            };

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntity(user.SecurityState);
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.Locked, result.StatusCode);

            var securityState = await _testHostFixture.AuthDbContext.SecurityStates
                .Where(ss => ss.UserId.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(securityState);
            Assert.Equal(0, securityState.FailedAttempts);
            Assert.True(securityState.IsLocked, "The account is not marked as locked.");
            Assert.NotNull(securityState.UnlockDate);

            var totalMinutes = (securityState.UnlockDate.Value - DateTime.UtcNow).TotalMinutes;
            Assert.True(totalMinutes >= accountLockSettings.SecondLockTimeInMinutes - 1 && totalMinutes <= accountLockSettings.SecondLockTimeInMinutes,
                $"The unlock date is not in range. Total minutes: {totalMinutes}");
        }

        [Fact]
        public async Task UpdateEmail_WhenPasswordIsCorrectButSomethingGoesWrong_ShouldReturnInternalServerErrorAndChangeNothing()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false);

            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(password)
                },
                Tokens = new List<Token>()
                {
                    new Token() 
                    { 
                        ValueHashed = _hashing.HashSha256(StringGenerator.GenerateUsingAsciiChars(10)), 
                        Purpose = ETokenPurpose.ResetPassword 
                    }
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var request = new UpdateEmailRequest()
            {
                NewEmail = EmailGenerator.Generate(),
                Password = password
            };

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntity(user.SecurityState);
            _testHostFixture.AuthDbContext.UntrackEntities(user.Tokens.ToArray());
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            EmailHelper.SetUseMessageBus(false);
            EmailHelper.SetSendEmailEventResult(false);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            var userResult = await _testHostFixture.AuthDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(userResult);
            Assert.Equal(email.ToLower(), userResult.Email);

            var tokens = await _testHostFixture.AuthDbContext.Tokens
                .Where(t => t.UserId.Equals(userId))
                .ToListAsync();
            Assert.Single(tokens);
            Assert.Equal(ETokenPurpose.ResetPassword, tokens[0].Purpose);
        }

        [Fact]
        public async Task UpdateEmail_WhenPasswordIsCorrect_ShouldReturnOkAndSaveNewEmailAndResetFailedAttempsAndCreateTokenAndSendEmail()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var password = PasswordGenerator.GenerateWithCustomRules(
                    length: 10,
                    requireDigit: true,
                    requireLowercase: false,
                    requireUppercase: false,
                    requireNonAlphanumeric: false);

            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(password),
                    IsEmailVerified = true,
                    FailedAttempts = 1
                },
                Tokens = new List<Token>()
                {
                    new Token() 
                    {
                        ValueHashed = _hashing.HashSha256(StringGenerator.GenerateUsingAsciiChars(10)),
                        Purpose = ETokenPurpose.ResetPassword 
                    },
                    new Token()
                    {
                        ValueHashed = _hashing.HashSha256(StringGenerator.GenerateUsingAsciiChars(10)),
                        Purpose = ETokenPurpose.EmailVerification
                    }
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var jwtToken = _jwt.GenerateJwtToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            });

            _testHostFixture.AddJwtBearerToken(jwtToken.AccessToken);

            var newEmail = EmailGenerator.Generate();
            var request = new UpdateEmailRequest()
            {
                NewEmail = newEmail,
                Password = password
            };

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntity(user.SecurityState);
            _testHostFixture.AuthDbContext.UntrackEntities(user.Tokens.ToArray());
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_updateEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var userResult = await _testHostFixture.AuthDbContext.Users
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(userResult);
            Assert.Equal(email.ToLower(), userResult.Email);
            Assert.Equal(newEmail.ToLower(), userResult.NewEmail);

            var securityState = await _testHostFixture.AuthDbContext.SecurityStates
                .Where(ss => ss.UserId.Equals(userId))
                .FirstOrDefaultAsync();
            Assert.NotNull(securityState);
            Assert.Equal(0, securityState.FailedAttempts);

            var tokens = await _testHostFixture.AuthDbContext.Tokens
                .Where(t => t.UserId.Equals(userId) && t.Purpose == ETokenPurpose.EmailVerification)
                .ToListAsync();
            Assert.Single(tokens);
        }
    }
}
