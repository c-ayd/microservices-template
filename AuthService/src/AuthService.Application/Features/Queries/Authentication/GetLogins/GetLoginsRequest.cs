using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AuthService.Application.Features.Queries.Authentication.GetLogins
{
    public class GetLoginsRequest : IAsyncRequest<ExecResult<GetLoginsResponse>>
    {
        public string? Password { get; set; }
    }
}
