using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.ExecutionResult.ClientError;
using Cayd.AspNetCore.ExecutionResult.Success;
using Cayd.AspNetCore.FlexLog;
using Cayd.AspNetCore.Mediator.Abstractions;
using Microsoft.Extensions.Options;
using AuthService.Application.Abstractions.Crypto;
using AuthService.Application.Abstractions.Messaging.Templates;
using AuthService.Application.Abstractions.UOW;
using AuthService.Application.Localization;
using AuthService.Application.Settings;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement.Enums;
using AuthService.Application.Abstractions.MessageBus.Publisher.Email;
using AuthService.Application.Dtos.MessageBus.Publisher.Email;

namespace AuthService.Application.Features.Commands.Authentication.Register
{
    public class RegisterHandler : IAsyncHandler<RegisterRequest, ExecResult<RegisterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashing _hashing;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly TokenLifetimesSettings _tokenLifetimesSettings;
        private readonly IEmailTemplates _emailTemplates;
        private readonly IEmailEventsPublisher _emailEventsPublisher;
        private readonly IFlexLogger<RegisterHandler> _flexLogger;

        public RegisterHandler(IUnitOfWork unitOfWork,
            IHashing hashing,
            ITokenGenerator tokenGenerator,
            IOptions<TokenLifetimesSettings> tokenLifetimesSettings,
            IEmailTemplates emailTemplates,
            IEmailEventsPublisher emailEventsPublisher,
            IFlexLogger<RegisterHandler> flexLogger)
        {
            _unitOfWork = unitOfWork;
            _hashing = hashing;
            _tokenGenerator = tokenGenerator;
            _tokenLifetimesSettings = tokenLifetimesSettings.Value;
            _emailTemplates = emailTemplates;
            _emailEventsPublisher = emailEventsPublisher;
            _flexLogger = flexLogger;
        }

        public async Task<ExecResult<RegisterResponse>> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            // Check if the user exists
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email!);
            if (user != null)
                return new ExecConflict("This email address already exists", AuthenticationLocalizationKeys.RegisterEmailExists);

            // Add a new user to the database
            var newUser = new User()
            {
                Email = request.Email,
                SecurityState = new SecurityState()
                {
                    PasswordHashed = _hashing.HashPassword(request.Password!)
                }
            };
            await _unitOfWork.Users.AddAsync(newUser);

            // Add a new email verification token to the database
            var emailVerificationTokenValue = _tokenGenerator.GenerateBase64UrlSafe();
            var emailVerificationExpirationTimeInHours = _tokenLifetimesSettings.EmailVerificationLifetimeInHours;
            var emailVerificationToken = new Token()
            {
                ValueHashed = _hashing.HashSha256(emailVerificationTokenValue),
                Purpose = ETokenPurpose.EmailVerification,
                ExpirationDate = DateTime.UtcNow.AddHours(emailVerificationExpirationTimeInHours),
                User = newUser
            };
            await _unitOfWork.Tokens.AddAsync(emailVerificationToken);

            // Save the new user and email verification token to the database
            await _unitOfWork.SaveChangesAsync();

            // Send a verification email
            var emailTemplate = _emailTemplates.GetEmailVerificationTemplate(emailVerificationTokenValue, emailVerificationExpirationTimeInHours);
            try
            {
                await _emailEventsPublisher.PublishSendEmailAsync(new SendEmailDto()
                {
                    CorrelationId = _flexLogger.LogContext.CorrelationId,
                    To = request.Email!,
                    Subject = emailTemplate.Subject!,
                    Body = emailTemplate.Body!,
                    IsBodyHtml = false  // NOTE: If the expected template is HTML, switch it to 'true'
                });
            }
            catch (Exception exception)
            {
                _flexLogger.LogError(exception.Message, exception);

                // The user is added to the database, however, the email could not be sent.
                return new ExecMultiStatus<RegisterResponse>(new RegisterResponse()
                {
                    UserId = newUser.Id
                },
                new
                {
                    Status = "The user has been created, but the verification email could not be sent",
                    LocalizationCode = AuthenticationLocalizationKeys.RegisterSucceededButSendingEmailFailed
                });
            }
            
            return new RegisterResponse()
            {
                UserId = newUser.Id
            };
        }
    }
}
