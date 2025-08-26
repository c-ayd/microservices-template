using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AuthService.Application.Features.Queries.Admin.GetUser
{
    public class GetUserRequest : IAsyncRequest<ExecResult<GetUserResponse>>
    {
        public Guid? Id { get; set; }
    }
}
