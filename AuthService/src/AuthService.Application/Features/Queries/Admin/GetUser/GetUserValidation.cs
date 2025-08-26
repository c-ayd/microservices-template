using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Queries.Admin.GetUser
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
