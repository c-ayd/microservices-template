using AdminService.Api.Settings;

namespace AdminService.Api.Configurations
{
    public static partial class Configurations
    {
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection(CorsSettings.SettingsKey).Get<CorsSettings>()!;

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(corsSettings.Origins.ToArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}
