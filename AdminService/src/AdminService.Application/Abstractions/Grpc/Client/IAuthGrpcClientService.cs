using AdminService.Application.Dtos.Grpc.Client;
using AdminService.Application.Features.Commands.DeleteUser;
using AdminService.Application.Features.Queries.GetUser;
using AdminService.Application.Features.Queries.GetUsers;

namespace AdminService.Application.Abstractions.Grpc.Client
{
    public interface IAuthGrpcClientService
    {
        Task<GetUsersResponse> GetUsersAsync(GetUsersRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default);
        Task<GetUserResponse> GetUserAsync(GetUserRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default);
        Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default);
    }
}
