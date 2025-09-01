using AdminService.Application.Abstractions.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Queries.GetUsers
{
    public class GetUsersRequest : IAsyncRequest<ExecResult<GetUsersResponse>>, IGrpcRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
