using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Queries.Admin.GetUsers
{
    public class GetUsersValidation : AbstractValidator<GetUsersRequest>
    {
        public GetUsersValidation()
        {
            RuleFor(_ => _.Page)
                .Cascade(CascadeMode.Stop)
                .PageValidation();

            RuleFor(_ => _.PageSize)
                .Cascade(CascadeMode.Stop)
                .PageSizeValidation();
        }
    }
}
