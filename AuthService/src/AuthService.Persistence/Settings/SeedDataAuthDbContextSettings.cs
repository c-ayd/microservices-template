using Cayd.AspNetCore.Settings;

namespace AuthService.Persistence.Settings
{
    public class SeedDataAuthDbContextSettings : ISettings
    {
        /**
         * NOTE: This class is used to read seed data from the configuration. Depending on specific needs,
         * the structure can be changed.
         */

        public static string SettingsKey => "SeedData:AuthDbContext";

        public required List<string> AdminEmails { get; set; }
    }
}
