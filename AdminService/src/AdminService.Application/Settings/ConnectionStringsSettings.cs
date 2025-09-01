using Cayd.AspNetCore.Settings;

namespace AdminService.Application.Settings
{
    public class ConnectionStringsSettings : ISettings
    {
        public static string SettingsKey => "ConnectionStrings";

        public required string Log { get; set; }
    }
}
