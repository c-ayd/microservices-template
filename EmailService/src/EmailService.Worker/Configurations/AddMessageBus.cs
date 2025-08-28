using EmailService.Worker.MessageBus;
using EmailService.Worker.MessageBus.Consumer.Email;
using EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail;
using EmailService.Worker.Settings;

namespace EmailService.Worker.Configurations
{
    public static partial class Configurations
    {
        public static void AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp =>
            {
                var connectionStrings = configuration.GetSection(ConnectionStringsSettings.SettingsKey).Get<ConnectionStringsSettings>()!;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new RabbitMqConnection(connectionStrings.RabbitMq, loggerFactory);
            });

            //NOTE: Add here all message bus events and message bus DLXs as singleton services
            services.AddSingleton<EmailEventsDlx>();

            services.AddSingleton<SendEmailEvent>();
        }
    }
}
