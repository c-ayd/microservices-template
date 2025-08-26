using Cayd.AspNetCore.Settings;

namespace AuthService.Application.Settings
{
    public class TokenLifetimesSettings : ISettings
    {
        public static string SettingsKey => "TokenLifetimes";

        public required int EmailVerificationLifetimeInHours { get; set; }
        public required int ResetPasswordLifetimeInHours { get; set; }
    }
}
