using Cayd.AspNetCore.Settings;

namespace AuthService.Infrastructure.Settings.Crypto
{
    public class AesGcmSettings : ISettings
    {
        public static string SettingsKey => "Crypto:AesGcm";

        public required string Key { get; set; }
    }
}
