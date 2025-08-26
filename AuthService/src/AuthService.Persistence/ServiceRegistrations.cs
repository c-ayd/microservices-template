using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Application.Abstractions.UOW;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.Settings;
using AuthService.Persistence.UOW;

namespace AuthService.Persistence
{
    public static class ServiceRegistrations
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connStrings = configuration.GetSection(ConnectionStringsSettings.SettingsKey).Get<ConnectionStringsSettings>()!;
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connStrings.App));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
