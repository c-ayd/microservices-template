using AuthService.Application.Dtos.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Mappings.Entities.UserManagement
{
    public static partial class UserManagementMappings
    {
        public static UserDto Map(User user)
            => new UserDto()
            {
                Id = user.Id,
                CreatedDate = user.CreatedDate,
                Email = user.Email,
                UpdatedDate = user.UpdatedDate,
                IsDeleted = user.IsDeleted,
                DeletedDate = user.DeletedDate
            };
    }
}
