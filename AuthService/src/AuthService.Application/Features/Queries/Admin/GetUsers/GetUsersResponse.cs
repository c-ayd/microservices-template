using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Features.Queries.Admin.GetUsers
{
    public class GetUsersResponse
    {
        public required List<User> Users { get; set; }
    }
}
