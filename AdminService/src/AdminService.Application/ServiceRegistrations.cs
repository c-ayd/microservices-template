using AdminService.Application.MediatorFlows;
using AdminService.Application.Validations;
using Cayd.AspNetCore.Mediator.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AdminService.Application
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
                config.AddTransientFlow(typeof(GrpcExceptionFlow<,>));
            });

            services.AddValidatorsFromAssembly(currentAssembly);
        }
    }
}
