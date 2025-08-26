using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AuthService.Application.Features.Commands.Authentication.RefreshToken
{
    public class RefreshTokenRequest : IAsyncRequest<ExecResult<RefreshTokenResponse>>
    {
    }
}
