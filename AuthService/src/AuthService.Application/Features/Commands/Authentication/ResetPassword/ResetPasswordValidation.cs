using FluentValidation;
using AuthService.Application.Localization;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Authentication.ResetPassword
{
    public class ResetPasswordValidation : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidation()
        {
            RuleFor(_ => _.Token)
                .NotEmpty()
                    .WithMessage("Token is null or empty")
                    .WithErrorCode(AuthenticationLocalizationKeys.TokenEmpty);

            RuleFor(_ => _.NewPassword)
                .Cascade(CascadeMode.Stop)
                .PasswordValidation();
        }
    }
}
