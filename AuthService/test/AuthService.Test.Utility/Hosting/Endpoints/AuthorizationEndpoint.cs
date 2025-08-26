using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AuthService.Test.Utility.Hosting.Policies;

namespace AuthService.Test.Utility.Hosting.Endpoints
{
    public static partial class TestEndpoints
    {
        public static void AddAuthorizationEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test/authorization/", () => Results.Ok())
                .RequireAuthorization(TestPolicy.PolicyName);
        }
    }
}
