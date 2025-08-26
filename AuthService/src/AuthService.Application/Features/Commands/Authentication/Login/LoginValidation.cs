using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Authentication.Login
{
    public class LoginValidation : AbstractValidator<LoginRequest>
    {
        public LoginValidation()
        {
            RuleFor(_ => _.Email)
                .Cascade(CascadeMode.Stop)
                .EmailValidation();

            RuleFor(_ => _.Password)
                .Cascade(CascadeMode.Stop)
                .PasswordValidation();
        }
    }
}
