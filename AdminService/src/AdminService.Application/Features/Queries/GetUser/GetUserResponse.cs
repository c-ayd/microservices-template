using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Dtos.Grpc.Client.Auth;

namespace AdminService.Application.Features.Queries.GetUser
{
    public class GetUserResponse : IGrpcResponse
    {
        public required UserGrpcDto User { get; set; }
        public SecurityStateGrpcDto? SecurityState { get; set; }
        public required List<RoleGrpcDto> Roles { get; set; }
        public required List<LoginGrpcDto> Logins { get; set; }
    }
}
