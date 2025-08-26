using Microsoft.Extensions.DependencyInjection;
using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Crypto;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Templates;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Crypto;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Messaging.Templates;

namespace AuthService.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IJwt, Jwt>();

            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<IHashing, Hashing>();
            services.AddSingleton<IEncryption, AesGcmEncryption>();

            services.AddSingleton<IEmailSender, Smtp>();
            services.AddTransient<IEmailTemplates, EmailTemplates>();
        }
    }
}
