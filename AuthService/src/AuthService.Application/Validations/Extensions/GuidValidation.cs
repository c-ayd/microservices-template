using FluentValidation;
using AuthService.Application.Localization;

namespace AuthService.Application.Validations.Extensions
{
    public static partial class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, Guid?> GuidValidation<T>(this IRuleBuilderInitial<T, Guid?> rule)
            => rule
                .NotNull()
                    .WithMessage("ID cannot be null")
                    .WithErrorCode(CommonLocalizationKeys.InvalidId)
                .NotEqual(Guid.Empty)
                    .WithMessage("ID cannot be empty")
                    .WithErrorCode(CommonLocalizationKeys.InvalidId);
    }
}
