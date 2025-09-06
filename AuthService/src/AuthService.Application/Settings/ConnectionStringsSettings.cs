using Cayd.AspNetCore.Settings;

namespace AuthService.Application.Settings
{
    public class ConnectionStringsSettings : ISettings
    {
        public static string SettingsKey => "ConnectionStrings";

        public required string Auth { get; set; }
        public required string RabbitMq { get; set; }
        public required string Log { get; set; }
    }
}
