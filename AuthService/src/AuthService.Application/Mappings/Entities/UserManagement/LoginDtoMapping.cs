using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Mappings.Entities.UserManagement
{
    public static partial class UserManagementMappings
    {
        public static LoginDto Map(Login login)
            => new LoginDto()
            {
                Id = login.Id,
                IpAddress = login.IpAddress,
                DeviceInfo = login.DeviceInfo,
                UpdatedDate = login.UpdatedDate,
                UserId = login.UserId
            };
    }
}
