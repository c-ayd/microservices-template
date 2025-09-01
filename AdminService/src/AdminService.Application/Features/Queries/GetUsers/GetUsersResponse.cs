using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Dtos.Grpc.Client.Auth;

namespace AdminService.Application.Features.Queries.GetUsers
{
    public class GetUsersResponse : IGrpcResponse
    {
        public required List<UserGrpcDto> Users { get; set; }
        public required int NumberOfNextPages { get; set; }
    }
}
