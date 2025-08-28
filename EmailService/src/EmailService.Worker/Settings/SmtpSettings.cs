using Cayd.AspNetCore.Settings;

namespace EmailService.Worker.Settings
{
    public class SmtpSettings : ISettings
    {
        public static string SettingsKey => "EmailSettings:Smtp";

        public required string Server { get; set; }
        public required int Port { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
    }
}
