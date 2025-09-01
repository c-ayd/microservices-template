using AdminService.Application.Abstractions.Http;

namespace AdminService.Api.Http
{
    public class RequestContext : IRequestContext
    {
        public Guid? UserId { get; set; }
        public string? JwtBearerToken { get; set; }
    }
}
