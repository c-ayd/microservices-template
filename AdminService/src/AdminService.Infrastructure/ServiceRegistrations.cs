using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Infrastructure.Grpc.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AdminService.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthGrpcClientService, AuthGrpcClientService>();
        }
    }
}
