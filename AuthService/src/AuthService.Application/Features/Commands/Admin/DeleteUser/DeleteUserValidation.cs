using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Commands.Admin.DeleteUser
{
    public class DeleteUserValidation : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserValidation()
        {
            RuleFor(_ => _.Id)
                .Cascade(CascadeMode.Stop)
                .GuidValidation();
        }
    }
}
