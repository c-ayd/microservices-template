using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Mappings.Entities.UserManagement
{
    public static partial class UserManagementMappings
    {
        public static RoleDto Map(Role role)
            => new RoleDto()
            {
                Id = role.Id,
                CreatedDate = role.CreatedDate,
                Name = role.Name
            };
    }
}
