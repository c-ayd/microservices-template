using AdminService.Api;
using AdminService.Api.Configurations;
using AdminService.Api.Filters;
using AdminService.Api.Logging.Sinks;
using AdminService.Api.Middlewares;
using AdminService.Application;
using AdminService.Application.Policies;
using AdminService.Application.Settings;
using AdminService.Infrastructure;
using Cayd.AspNetCore.FlexLog.DependencyInjection;
using Cayd.AspNetCore.Settings.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

builder.Services.ConfigureDataProtection(builder.Configuration);

var app = builder.Build();

app.AddMiddlewares();

app.MapControllers();

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
        services.AddInfrastructureServices();
        services.AddApplicationServices();

        services.AddSettingsFromAssemblies(configuration,
            Assembly.GetAssembly(typeof(Program))!,
            Assembly.GetAssembly(typeof(AdminService.Infrastructure.ServiceRegistrations))!,
            Assembly.GetAssembly(typeof(AdminService.Application.ServiceRegistrations))!);

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
    }
}
