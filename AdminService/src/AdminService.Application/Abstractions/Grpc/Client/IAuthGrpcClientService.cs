using AdminService.Application.Dtos.Grpc.Client;
using AdminService.Application.Features.Queries.GetUsers;

namespace AdminService.Application.Abstractions.Grpc.Client
{
    public interface IAuthGrpcClientService
    {
        Task<GetUsersResponse> GetUsersAsync(GetUsersRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default);
    }
}
