using Cayd.AspNetCore.Settings;

namespace AuthService.Persistence.Settings
{
    public class ConnectionStringsSettings : ISettings
    {
        public static string SettingsKey => "ConnectionStrings";

        public required string App { get; set; }
        public required string Log { get; set; }
    }
}
