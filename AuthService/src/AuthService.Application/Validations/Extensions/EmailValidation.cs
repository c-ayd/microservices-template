using FluentValidation;
using AuthService.Application.Localization;
using AuthService.Application.Validations.Constants.Entities.UserManagement;

namespace AuthService.Application.Validations.Extensions
{
    public static partial class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string?> EmailValidation<T>(this IRuleBuilderInitial<T, string?> rule)
            => rule
                .NotEmpty()
                    .WithMessage("Email is null or empty")
                    .WithErrorCode(AuthenticationLocalizationKeys.EmailRequired)
                .MaximumLength(UserConstants.EmailMaxLength)
                    .WithMessage("Email address is too long")
                    .WithErrorCode(AuthenticationLocalizationKeys.EmailTooLong)
                .Matches(UserConstants.EmailRegex)
                    .WithMessage("Email is invalid")
                    .WithErrorCode(AuthenticationLocalizationKeys.EmailInvalid);
    }
}
