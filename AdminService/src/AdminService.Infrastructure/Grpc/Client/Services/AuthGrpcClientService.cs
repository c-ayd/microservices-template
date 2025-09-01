using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Dtos.Grpc.Client;
using AdminService.Application.Dtos.Grpc.Client.Auth;
using AdminService.Application.Features.Commands.DeleteUser;
using AdminService.Application.Features.Queries.GetUser;
using AdminService.Application.Features.Queries.GetUsers;
using AdminService.Application.Settings;
using AdminService.Infrastructure.Grpc.Client.Interceptors;
using Cayd.AspNetCore.FlexLog.Options;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace AdminService.Infrastructure.Grpc.Client.Services
{
    public class AuthGrpcClientService : IAuthGrpcClientService, IAsyncDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly CallInvoker _callInvoker;
        private readonly string _correlationIdKey;
    
        public AuthGrpcClientService(IOptions<ConnectionStringsSettings> connectionStringsSettings,
            IOptions<FlexLogOptions> loggingOptions)
        {
            _channel = GrpcChannel.ForAddress(connectionStringsSettings.Value.Grpc.Auth);
            _callInvoker = _channel.Intercept(new GlobalExceptionInterceptor());
            _correlationIdKey = loggingOptions.Value.LogDetails!.Headers!.CorrelationIdKey!;
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.ShutdownAsync();
            _channel.Dispose();
        }

        private Metadata? BuildMetadata(GrpcMetadataDto? dto)
        {
            if (dto == null)
                return null;

            var metadata = new Metadata
            {
                { GrpcHeaderKeys.UserId, dto.UserId.ToString() },
                { GrpcHeaderKeys.Authorization, dto.JwtBearerToken },
                { GrpcHeaderKeys.Timestamp, DateTime.UtcNow.ToString("o") },
                { _correlationIdKey, dto.CorrelationId.ToString() }
            };

            return metadata;
        }

        public async Task<GetUsersResponse> GetUsersAsync(GetUsersRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default)
        {
            var client = new AdminGrpcService.AdminGrpcServiceClient(_callInvoker);
            var response = await client.GetUsersAsync(new GetUsersGrpcRequest()
            {
                Page = request.Page!.Value,
                PageSize = request.PageSize!.Value
            }, BuildMetadata(metadataDto), cancellationToken: cancellationToken);

            var users = new List<UserGrpcDto>();
            foreach (var user in response.Users)
            {
                users.Add(new UserGrpcDto()
                {
                    Id = user.Id,
                    CreatedDate = user.CreatedDate.ToDateTime(),
                    Email = user.Email,
                    UpdatedDate = user.UpdatedDate?.ToDateTime(),
                    IsDeleted = user.IsDeleted,
                    DeletedDate = user.DeletedDate?.ToDateTime()
                });
            }

            return new GetUsersResponse()
            {
                Users = users,
                NumberOfNextPages = response.NumberOfNextPages
            };
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default)
        {
            var client = new AdminGrpcService.AdminGrpcServiceClient(_callInvoker);
            var response = await client.GetUserAsync(new GetUserGrpcRequest()
            {
                Id = request.Id!.Value.ToString()
            }, BuildMetadata(metadataDto), cancellationToken: cancellationToken);
            
            var user = new UserGrpcDto()
            {
                Id = response.User.Id,
                CreatedDate = response.User.CreatedDate.ToDateTime(),
                Email = response.User.Email,
                UpdatedDate = response.User.UpdatedDate?.ToDateTime(),
                IsDeleted = response.User.IsDeleted,
                DeletedDate = response.User.DeletedDate?.ToDateTime()
            };
            
            SecurityStateGrpcDto? securityState = null;
            if (response.SecurityState != null)
            {
                securityState = new SecurityStateGrpcDto();
                securityState.FailedAttempts = response.SecurityState.FailedAttemps;
                securityState.IsEmailVerified = response.SecurityState.IsEmailVerified;
                securityState.IsLocked = response.SecurityState.IsLocked;
                securityState.UnlockDate = response.SecurityState.UnlockDate?.ToDateTime();
            }

            var roles = new List<RoleGrpcDto>();
            foreach (var role in response.Roles)
            {
                roles.Add(new RoleGrpcDto()
                {
                    Name = role.Name
                });
            }

            var logins = new List<LoginGrpcDto>();
            foreach (var login in response.Logins)
            {
                logins.Add(new LoginGrpcDto()
                {
                    IpAddress = login.IpAddress,
                    DeviceInfo = login.DeviceInfo,
                    UpdatedDate = login.UpdatedDate?.ToDateTime()
                });
            }
            
            return new GetUserResponse()
            {
                User = user,
                SecurityState = securityState,
                Roles = roles,
                Logins = logins
            };
        }

        public async Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request, GrpcMetadataDto? metadataDto = null, CancellationToken cancellationToken = default)
        {
            var client = new AdminGrpcService.AdminGrpcServiceClient(_callInvoker);
            await client.DeleteUserAsync(new DeleteUserGrpcRequest()
            {
                Id = request.Id!.Value.ToString()
            }, BuildMetadata(metadataDto), cancellationToken: cancellationToken);

            return new DeleteUserResponse();
        }
    }
}
