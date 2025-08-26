using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AuthService.Application.Policies;

namespace AuthService.Test.Utility.Hosting.Endpoints
{
    public static partial class TestEndpoints
    {
        public static void AddEmailVerifiactionPolicyEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test/email-verification-policy", () => Results.Ok())
                .RequireAuthorization(EmailVerificationPolicy.PolicyName);
        }
    }
}
