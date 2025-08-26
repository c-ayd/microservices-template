using AuthService.Api.Http;
using AuthService.Application.Abstractions.Http;

namespace AuthService.Api
{
    public static class ServiceRegistrations
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestContext, RequestContext>();
        }
    }
}
