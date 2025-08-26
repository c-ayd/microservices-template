using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AuthService.Application.Features.Commands.Authentication.DeleteLogin
{
    public class DeleteLoginRequest : IAsyncRequest<ExecResult<DeleteLoginResponse>>
    {
        public Guid? Id { get; set; }
    }
}
