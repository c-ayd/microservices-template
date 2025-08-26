using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Application.Features.Queries.Authentication.GetLogins;
using AuthService.Application.Mappings.Entities.UserManagement;

namespace AuthService.Application.Mappings.Controllers.Authentication
{
    public static partial class AuthenticationMappings
    {
        public static List<LoginDto> Map(GetLoginsResponse response)
        {
            var result = new List<LoginDto>();

            foreach (var login in response.Logins)
            {
                result.Add(UserManagementMappings.Map(login));
            }

            return result;
        }
    }
}
