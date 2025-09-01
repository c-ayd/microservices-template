namespace AdminService.Application.Abstractions.Http
{
    public interface IRequestContext
    {
        Guid? UserId { get; }
        string? JwtBearerToken { get; }
    }
}
