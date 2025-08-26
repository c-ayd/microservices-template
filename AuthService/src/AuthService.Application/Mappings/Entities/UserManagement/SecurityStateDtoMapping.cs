using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Mappings.Entities.UserManagement
{
    public static partial class UserManagementMappings
    {
        public static SecurityStateDto Map(SecurityState securityState)
            => new SecurityStateDto()
            {
                IsEmailVerified = securityState.IsEmailVerified,
                FailedAttempts = securityState.FailedAttempts,
                IsLocked = securityState.IsLocked,
                UnlockDate = securityState.UnlockDate,
                UserId = securityState.UserId
            };
    }
}
