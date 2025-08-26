using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AuthService.Application.Features.Commands.Authentication.VerifyEmail
{
    public class VerifyEmailRequest : IAsyncRequest<ExecResult<VerifyEmailResponse>>
    {
        public string? Token { get; set; }
    }
}
