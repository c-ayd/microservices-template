using FluentValidation;
using AdminService.Application.Validations.Extensions;

namespace AdminService.Application.Features.Queries.GetUsers
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
