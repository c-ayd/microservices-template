using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Authentication.DeleteLogin
{
    public class DeleteLoginValidation : AbstractValidator<DeleteLoginRequest>
    {
        public DeleteLoginValidation()
        {
            RuleFor(_ => _.Id)
                .Cascade(CascadeMode.Stop)
                .GuidValidation();
        }
    }
}
