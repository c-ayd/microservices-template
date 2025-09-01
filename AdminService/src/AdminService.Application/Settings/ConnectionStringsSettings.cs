using Cayd.AspNetCore.Settings;

namespace AdminService.Application.Settings
{
    public class ConnectionStringsSettings : ISettings
    {
        public static string SettingsKey => "ConnectionStrings";

        public required GrpcConnections Grpc { get; set; }
        public required string Log { get; set; }

        public class GrpcConnections
        {
            public required string Auth { get; set; }
        }
    }
}
