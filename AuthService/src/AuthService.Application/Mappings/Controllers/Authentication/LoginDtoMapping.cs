using AuthService.Application.Dtos.Controllers.Authentication;
using AuthService.Application.Features.Commands.Authentication.Login;

namespace AuthService.Application.Mappings.Controllers.Authentication
{
    public static partial class AuthenticationMappings
    {
        public static LoginDto Map(LoginResponse response)
            => new LoginDto()
                {
                    AccessToken = response.AccessToken
                };
    }
}
