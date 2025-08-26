using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AuthService.Application.Policies;

namespace AuthService.Test.Utility.Hosting.Endpoints
{
    public static partial class TestEndpoints
    {
        public static void AddAdminPolicyEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test/admin-policy", () => Results.Ok())
                .RequireAuthorization(AdminPolicy.PolicyName);
        }
    }
}
