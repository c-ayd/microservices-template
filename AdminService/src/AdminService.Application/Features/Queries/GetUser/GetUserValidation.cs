using FluentValidation;
using AdminService.Application.Validations.Extensions;

namespace AdminService.Application.Features.Queries.GetUser
{
    public class GetUserValidation : AbstractValidator<GetUserRequest>
    {
        public GetUserValidation()
        {
            RuleFor(_ => _.Id)
                .Cascade(CascadeMode.Stop)
                .GuidValidation();
        }
    }
}
