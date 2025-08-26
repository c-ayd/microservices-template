using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Test.Utility.Hosting.Endpoints
{
    public static partial class TestEndpoints
    {
        public static void AddExceptionEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test/exception", () =>
            {
                throw new Exception("Test exception");
            });
        }
    }
}
