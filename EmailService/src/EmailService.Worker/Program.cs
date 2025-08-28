using Cayd.AspNetCore.Settings.DependencyInjection;
using EmailService.Worker.Abstractions.Messaging;
using EmailService.Worker.Services.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices(builder.Configuration);

var host = builder.Build();

host.Run();

public static partial class Program
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettingsFromAssembly(configuration, typeof(Program).Assembly);

        services.AddSingleton<IEmailSender, Smtp>();
    }
}
