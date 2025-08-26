using System.Security.Claims;
using AuthService.Application.Dtos.Authentication;

namespace AuthService.Application.Abstractions.Authentication
{
    public interface IJwt
    {
        JwtTokenDto GenerateJwtToken();
        JwtTokenDto GenerateJwtToken(ICollection<Claim> claims);
        JwtTokenDto GenerateJwtToken(ICollection<Claim> claims, DateTime notBefore);
        JwtTokenDto GenerateJwtToken(DateTime notBefore);
    }
}
