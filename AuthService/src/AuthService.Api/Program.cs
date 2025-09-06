using Cayd.AspNetCore.FlexLog.DependencyInjection;
using Cayd.AspNetCore.Settings.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using AuthService.Api;
using AuthService.Api.Configurations;
using AuthService.Api.Filters;
using AuthService.Api.Logging.Sinks;
using AuthService.Api.Middlewares;
using AuthService.Application;
using AuthService.Application.Policies;
using AuthService.Infrastructure;
using AuthService.Persistence;
using AuthService.Persistence.SeedData;
using AuthService.Application.Settings;
using AuthService.Infrastructure.Grpc.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

builder.Services.ConfigureRateLimiter(builder.Configuration);
builder.Services.ConfigureDataProtection(builder.Configuration);

var app = builder.Build();

await app.Services.SeedDataAuthDbContext(app.Configuration);

app.AddMiddlewares();

app.UseRateLimiter();

app.MapControllers();
app.MapGrpcService<AdminGrpcServerService>();

app.Run();

public static partial class Program 
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.ConfigureJwtBearer(configuration);
            });

        services.AddAuthorization(config =>
        {
            config.AddPolicy(AdminPolicy.PolicyName,
                p => p.RequireRole(AdminPolicy.RoleName));

            config.AddPolicy(EmailVerificationPolicy.PolicyName,
                p => p.RequireClaim(EmailVerificationPolicy.ClaimName, EmailVerificationPolicy.ClaimValue));
        });

        services.AddControllers(config =>
        {
            config.Filters.Add(new ProblemDetailsPopulaterFilter());
        }).AddApplicationPart(typeof(Program).Assembly);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.ConfigureInvalidModelStateResponse();
        });

        services.AddFlexLog(configuration, config =>
        {
            var connectionStrings = configuration.GetSection(ConnectionStringsSettings.SettingsKey).Get<ConnectionStringsSettings>()!;

            config.AddSink(new DatabaseSink(connectionStrings.Log));
        });

        services.AddPresentationServices();
        services.AddPersistenceServices(configuration);
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();

        services.AddSettingsFromAssemblies(configuration,
            Assembly.GetAssembly(typeof(Program))!,
            Assembly.GetAssembly(typeof(AuthService.Persistence.ServiceRegistrations))!,
            Assembly.GetAssembly(typeof(AuthService.Infrastructure.ServiceRegistrations))!,
            Assembly.GetAssembly(typeof(AuthService.Application.ServiceRegistrations))!);

        services.AddLocalization(config => config.ResourcesPath = "Resources");

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.ConfigureCors(configuration);
    }

    public static void AddMiddlewares(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();

        app.UseFlexLog();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestContextPopulator>();

        app.UseRequestLocalization(new RequestLocalizationOptions()
            .SetDefaultCulture("en")
            .AddSupportedUICultures(
                "en",
                "de"
            ));
    }
}
