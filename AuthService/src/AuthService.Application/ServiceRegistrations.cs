using Cayd.AspNetCore.Mediator.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AuthService.Application.Validations;

namespace AuthService.Application
{
    public static class ServiceRegistrations
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly()!;

            services.AddMediator(config =>
            {
                config.AddHandlersFromAssembly(currentAssembly);

                config.AddTransientFlow(typeof(MediatorValidationFlow<,>));
            });

            services.AddValidatorsFromAssembly(currentAssembly);
        }
    }
}
