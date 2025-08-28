using Cayd.AspNetCore.Settings;

namespace EmailService.Worker.Settings
{
    public class ConnectionStringsSettings : ISettings
    {
        public static string SettingsKey => "ConnectionStrings";

        public required string RabbitMq { get; set; }
        public required string Log { get; set; }
    }
}
