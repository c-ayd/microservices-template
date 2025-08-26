namespace AuthService.Application.Dtos.Controllers.Authentication
{
    public class RefreshTokenDto
    {
        public required string AccessToken { get; set; }
    }
}
