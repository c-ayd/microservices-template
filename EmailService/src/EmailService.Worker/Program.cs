using Cayd.AspNetCore.FlexLog.DependencyInjection;
using Cayd.AspNetCore.Settings.DependencyInjection;
using EmailService.Worker.Abstractions.Messaging;
using EmailService.Worker.Configurations;
using EmailService.Worker.Logging.Sinks;
using EmailService.Worker.Services.Messaging;
using EmailService.Worker.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices(builder.Configuration);

var host = builder.Build();

await host.StartMessageBus();

host.Run();

public static partial class Program
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettingsFromAssembly(configuration, typeof(Program).Assembly);

        services.AddSingleton<IEmailSender, Smtp>();

        services.AddFlexLog(configuration, config =>
        {
            var connectionStrings = configuration.GetSection(ConnectionStringsSettings.SettingsKey).Get<ConnectionStringsSettings>()!;

            config.AddSink(new DatabaseSink(connectionStrings.Log));
        });

        services.AddMessageBus(configuration);
    }
}
