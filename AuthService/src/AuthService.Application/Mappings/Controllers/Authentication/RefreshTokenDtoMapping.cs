using AuthService.Application.Dtos.Controllers.Authentication;
using AuthService.Application.Features.Commands.Authentication.RefreshToken;

namespace AuthService.Application.Mappings.Controllers.Authentication
{
    public static partial class AuthenticationMappings
    {
        public static RefreshTokenDto Map(RefreshTokenResponse response)
            => new RefreshTokenDto()
                {
                    AccessToken = response.AccessToken
                };
    }
}
