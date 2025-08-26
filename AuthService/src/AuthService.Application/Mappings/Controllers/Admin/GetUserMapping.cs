using AuthService.Application.Dtos.Controllers.Admin;
using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Application.Features.Queries.Admin.GetUser;
using AuthService.Application.Mappings.Entities.UserManagement;

namespace AuthService.Application.Mappings.Controllers.Admin
{
    public static partial class AdminMappings
    {
        public static GetUserDto? Map(GetUserResponse? response)
        {
            if (response == null)
                return null;

            var roles = new List<RoleDto>();
            foreach (var role in response.User.Roles)
            {
                roles.Add(UserManagementMappings.Map(role));
            }

            var logins = new List<LoginDto>();
            foreach (var login in response.User.Logins)
            {
                logins.Add(UserManagementMappings.Map(login));
            }

            return new GetUserDto()
            {
                User = UserManagementMappings.Map(response.User),
                SecurityState = response.User.SecurityState != null ? 
                    UserManagementMappings.Map(response.User.SecurityState) : null,
                Roles = roles,
                Logins = logins
            };
        }
    }
}
