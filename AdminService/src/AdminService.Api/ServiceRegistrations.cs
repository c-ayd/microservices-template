using AdminService.Api.Http;
using AdminService.Application.Abstractions.Http;

namespace AdminService.Api
{
    public static class ServiceRegistrations
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestContext, RequestContext>();
        }
    }
}
