using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using AuthService.Application.Features.Commands.Authentication.SendEmail;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement.Enums;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.TestValues;

namespace AuthService.Test.Integration.Api.Controllers.Authentication
{
    public partial class AuthenticationControllerTest
    {
        private const string _sendEmailEndpoint = "/auth/send-email";

        [Theory]
        [MemberData(nameof(TestValues.GetInvalidEmails), MemberType = typeof(TestValues))]
        public async Task SendEmail_WhenEmailIsInvalid_ShouldReturnBadRequest(string? email)
        {
            // Arrange
            var request = new SendEmailRequest()
            {
                Email = email,
                Purpose = ETokenPurpose.EmailVerification
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WhenPurposeIsInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SendEmailRequest()
            {
                Email = EmailGenerator.Generate(),
                Purpose = (ETokenPurpose)(-1)
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WhenEmailDoesNotExist_ShouldReturnOk()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var request = new SendEmailRequest()
            {
                Email = email,
                Purpose = ETokenPurpose.EmailVerification
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WhenEmailPurposeIsEmailVerificationAndEmailIsAlreadyVerified_ShouldReturnConflict()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    IsEmailVerified = true
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var request = new SendEmailRequest()
            {
                Email = email,
                Purpose = ETokenPurpose.EmailVerification
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WhenEmailIsNotSent_ShouldReturnInternalServerErrorAndNotCreateToken()
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    IsEmailVerified = false
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var request = new SendEmailRequest()
            {
                Email = email,
                Purpose = ETokenPurpose.EmailVerification
            };

            EmailHelper.SetUseMessageBus(false);
            EmailHelper.SetSendEmailEventResult(false);

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            var tokens = await _testHostFixture.AuthDbContext.Tokens
                .Where(t => t.UserId.Equals(user.Id))
                .ToListAsync();
            Assert.Empty(tokens);
        }

        [Theory]
        [InlineData(ETokenPurpose.EmailVerification)]
        [InlineData(ETokenPurpose.ResetPassword)]
        public async Task SendEmail_WhenEmailIsSent_ShouldReturnOkAndSendEmailAndCreateNewTokenAndDeleteOtherSameTypeTokens(ETokenPurpose purpose)
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email,
                SecurityState = new SecurityState()
                {
                    IsEmailVerified = false
                },
                Tokens = new List<Token>()
                {
                    new Token() 
                    {
                        ValueHashed = _hashing.HashSha256(StringGenerator.GenerateUsingAsciiChars(10)),
                        Purpose = ETokenPurpose.EmailVerification,
                        ExpirationDate = DateTime.UtcNow.AddDays(1)
                    },
                    new Token()
                    {
                        ValueHashed = _hashing.HashSha256(StringGenerator.GenerateUsingAsciiChars(10)),
                        Purpose = ETokenPurpose.ResetPassword,
                        ExpirationDate = DateTime.UtcNow.AddDays(1)
                    }
                }
            };

            await _testHostFixture.AuthDbContext.Users.AddAsync(user);
            await _testHostFixture.AuthDbContext.SaveChangesAsync();

            var userId = user.Id;
            _testHostFixture.AuthDbContext.UntrackEntities(user.Tokens.ToArray());
            _testHostFixture.AuthDbContext.UntrackEntity(user);

            var request = new SendEmailRequest()
            {
                Email = email,
                Purpose = purpose
            };

            // Act
            var result = await _testHostFixture.Client.PostAsJsonAsync(_sendEmailEndpoint, request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var tokens = await _testHostFixture.AuthDbContext.Tokens
                .Where(t => t.UserId.Equals(userId))
                .ToListAsync();
            Assert.Equal(2, tokens.Count);
            if (tokens[0].Purpose == ETokenPurpose.EmailVerification)
            {
                Assert.Equal(ETokenPurpose.ResetPassword, tokens[1].Purpose);
            }
            else
            {
                Assert.Equal(ETokenPurpose.EmailVerification, tokens[1].Purpose);
            }
        }
    }
}
