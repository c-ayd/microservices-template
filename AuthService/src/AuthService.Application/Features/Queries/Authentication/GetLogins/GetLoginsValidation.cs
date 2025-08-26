using FluentValidation;
using AuthService.Application.Validations.Extensions;

namespace AuthService.Application.Features.Queries.Authentication.GetLogins
{
    public class GetLoginsValidation : AbstractValidator<GetLoginsRequest>
    {
        public GetLoginsValidation()
        {
            RuleFor(_ => _.Password)
                .Cascade(CascadeMode.Stop)
                .PasswordValidation();
        }
    }
}
