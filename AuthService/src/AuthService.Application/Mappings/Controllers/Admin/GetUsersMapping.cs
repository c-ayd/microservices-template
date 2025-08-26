using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Application.Features.Queries.Admin.GetUsers;
using AuthService.Application.Mappings.Entities.UserManagement;

namespace AuthService.Application.Mappings.Controllers.Admin
{
    public static partial class AdminMappings
    {
        public static List<UserDto> Map(GetUsersResponse? response)
        {
            var result = new List<UserDto>();

            if (response == null)
                return result;

            foreach (var user in response.Users)
            {
                result.Add(UserManagementMappings.Map(user));
            }

            return result;
        }
    }
}
