using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Authentication.UpdatePassword
{
    public class UpdatePasswordValidation : AbstractValidator<UpdatePasswordRequest>
    {
        public UpdatePasswordValidation()
        {
            RuleFor(_ => _.NewPassword)
                .Cascade(CascadeMode.Stop)
                .PasswordValidation();

            RuleFor(_ => _.Password)
                .Cascade(CascadeMode.Stop)
                .PasswordValidation();
        }
    }
}
