using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Application.Features.Queries.Authentication.GetLogins
{
    public class GetLoginsResponse
    {
        public required ICollection<Login> Logins { get; set; }
    }
}
