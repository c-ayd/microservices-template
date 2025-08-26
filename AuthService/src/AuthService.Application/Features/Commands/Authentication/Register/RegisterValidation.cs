using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Authentication.Register
{
    public class RegisterValidation : AbstractValidator<RegisterRequest>
    {
        public RegisterValidation()
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
