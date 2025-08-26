using Cayd.AspNetCore.Settings;

namespace AuthService.Api.Settings
{
    public class DataProtectionSettings : ISettings
    {
        public static string SettingsKey => "DataProtection";

        public required string FilePath { get; set; }
    }
}
