using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AuthService.Application.Abstractions.Http;

namespace AuthService.Test.Utility.Hosting.Endpoints
{
    public static partial class TestEndpoints
    {
        public static void AddEmptyEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test/request-context", (IRequestContext requestContext) =>
            {
                return Results.Ok(requestContext);
            });
        }
    }
}
