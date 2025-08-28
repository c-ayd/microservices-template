using Microsoft.Extensions.DependencyInjection;
using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Crypto;
using AuthService.Application.Abstractions.Messaging.Templates;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Crypto;
using AuthService.Infrastructure.Messaging.Templates;
using AuthService.Application.Abstractions.MessageBus.Publisher.Email;
using AuthService.Infrastructure.MessageBus.Publisher.Email;
using AuthService.Infrastructure.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJwt, Jwt>();

            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<IHashing, Hashing>();
            services.AddSingleton<IEncryption, AesGcmEncryption>();

            services.AddTransient<IEmailTemplates, EmailTemplates>();

            services.AddSingleton(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new RabbitMqConnection(configuration.GetConnectionString("RabbitMq")!, loggerFactory);
            });

            services.AddSingleton<IEmailEventsPublisher, EmailEventsPublisher>();
        }
    }
}
