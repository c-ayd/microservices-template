using AdminService.Api.Http;
using AdminService.Application.Abstractions.Http;
using System.Security.Claims;

namespace AdminService.Api.Middlewares
{
    public class RequestContextPopulator
    {
        private readonly RequestDelegate _next;

        public RequestContextPopulator(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestContext = (RequestContext)context.RequestServices.GetRequiredService<IRequestContext>();
            if (requestContext != null)
            {
                requestContext.UserId = GetUserId(context.User);
                requestContext.JwtBearerToken = GetJwtBearerToken(context);
            }

            await _next(context);
        }

        private Guid? GetUserId(ClaimsPrincipal user)
        {
            var nameIdentifier = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (nameIdentifier == null)
                return null;

            return Guid.TryParse(nameIdentifier, out var userId) ? userId : null;
        }

        private string? GetJwtBearerToken(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var jwtBearerToken))
                return null;

            return jwtBearerToken.ToString();
        }
    }
}
