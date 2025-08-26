using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Features.Queries.Admin.GetUser
{
    public class GetUserResponse
    {
        public required User User { get; set; }
    }
}
