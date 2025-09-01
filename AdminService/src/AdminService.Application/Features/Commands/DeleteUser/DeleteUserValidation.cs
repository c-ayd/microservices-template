using FluentValidation;
using AdminService.Application.Validations.Extensions;

namespace AdminService.Application.Features.Commands.DeleteUser
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
