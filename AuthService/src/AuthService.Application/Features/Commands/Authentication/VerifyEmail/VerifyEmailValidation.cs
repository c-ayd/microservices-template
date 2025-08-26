using FluentValidation;
using AuthService.Application.Localization;

namespace AuthService.Application.Features.Commands.Authentication.VerifyEmail
{
    public class VerifyEmailValidation : AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailValidation()
        {
            RuleFor(_ => _.Token)
                .NotEmpty()
                    .WithMessage("Token is null or empty")
                    .WithErrorCode(AuthenticationLocalizationKeys.TokenEmpty);
        }
    }
}
