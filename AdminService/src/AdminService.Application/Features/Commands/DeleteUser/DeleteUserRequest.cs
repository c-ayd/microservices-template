using AdminService.Application.Abstractions.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Commands.DeleteUser
{
    public class DeleteUserRequest : IAsyncRequest<ExecResult<DeleteUserResponse>>, IGrpcRequest
    {
        public Guid? Id { get; set; }
    }
}
