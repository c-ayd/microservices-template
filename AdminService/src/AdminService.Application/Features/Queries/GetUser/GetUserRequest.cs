using AdminService.Application.Abstractions.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Queries.GetUser
{
    public class GetUserRequest : IAsyncRequest<ExecResult<GetUserResponse>>, IGrpcRequest
    {
        public Guid? Id { get; set; }
    }
}
