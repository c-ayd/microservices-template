using System.Net;

namespace AuthService.Application.Abstractions.Http
{
    public interface IRequestContext
    {
        public static List<string> DefaultPreferredLanguages = new List<string>() { "en" };

        Guid? UserId { get; }
        bool? IsEmailVerified { get; }
        string? RefreshToken { get; }
        IPAddress? IpAddress { get; }
        string? DeviceInfo { get; }
        List<string> PreferredLanguages { get; }
    }
}
