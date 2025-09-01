using AuthService.Application.Abstractions.UOW;
using AuthService.Application.Policies;
using AuthService.Application.Validations.Constants;
using Cayd.AspNetCore.FlexLog;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Infrastructure.Grpc.Server.Services
{
    [Authorize(Roles = AdminPolicy.RoleName)]
    public class AdminGrpcServerService : AdminGrpcService.AdminGrpcServiceBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlexLogger<AdminGrpcServerService> _flexLogger;

        public AdminGrpcServerService(IUnitOfWork unitOfWork,
            IFlexLogger<AdminGrpcServerService> flexLogger)
        {
            _unitOfWork = unitOfWork;
            _flexLogger = flexLogger;
        }

        public override async Task<GetUsersGrpcResponse> GetUsers(GetUsersGrpcRequest request, ServerCallContext context)
        {
            var response = new GetUsersGrpcResponse();

            var (users, numberOfNextPages) = await _unitOfWork.Users.GetAllAsync(request.Page, request.PageSize, PaginationConstants.MaxNumberOfNextPages, context.CancellationToken);
            if (users.Count == 0)
                return response;

            foreach (var user in users)
            {
                response.Users.Add(new UserModel()
                {
                    Id = user.Id.ToString(),
                    CreatedDate = Timestamp.FromDateTime(user.CreatedDate),
                    Email = user.Email,
                    IsDeleted = user.IsDeleted,
                    DeletedDate = user.DeletedDate != null ? Timestamp.FromDateTime(user.DeletedDate.Value) : null,
                    UpdatedDate = user.UpdatedDate != null ? Timestamp.FromDateTime(user.UpdatedDate.Value) : null
                });
            }

            response.NumberOfNextPages = numberOfNextPages;

            return response;
        }

        public override async Task<GetUserGrpcResponse> GetUser(GetUserGrpcRequest request, ServerCallContext context)
        {
            var user = await _unitOfWork.Users.GetWithFullContextByIdAsync(Guid.Parse(request.Id), context.CancellationToken);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User could not be found"));

            var response = new GetUserGrpcResponse();

            response.User = new UserModel()
            {
                Id = user.Id.ToString(),
                CreatedDate = Timestamp.FromDateTime(user.CreatedDate),
                Email = user.Email,
                IsDeleted = user.IsDeleted,
                DeletedDate = user.DeletedDate != null ? Timestamp.FromDateTime(user.DeletedDate.Value) : null,
                UpdatedDate = user.UpdatedDate != null ? Timestamp.FromDateTime(user.UpdatedDate.Value) : null
            };

            if (user.SecurityState != null)
            {
                response.SecurityState = new SecurityStateModel()
                {
                    FailedAttemps = user.SecurityState.FailedAttempts,
                    IsEmailVerified = user.SecurityState.IsEmailVerified,
                    IsLocked = user.SecurityState.IsLocked,
                    UnlockDate = user.SecurityState.UnlockDate != null ? Timestamp.FromDateTime(user.SecurityState.UnlockDate.Value) : null
                };
            }

            foreach (var role in user.Roles)
            {
                response.Roles.Add(new RoleModel()
                {
                    Name = role.Name
                });
            }

            foreach (var login in user.Logins)
            {
                response.Logins.Add(new LoginModel()
                {
                    IpAddress = login.IpAddress?.ToString(),
                    DeviceInfo = login.DeviceInfo,
                    UpdatedDate = login.UpdatedDate != null ? Timestamp.FromDateTime(login.UpdatedDate.Value) : null
                });
            }

            return response;
        }

        public override async Task<Empty> DeleteUser(DeleteUserGrpcRequest request, ServerCallContext context)
        {
            var user = await _unitOfWork.Users.GetByIdWithSecurityStateAsync(Guid.Parse(request.Id));
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User could not be found"));
            
            if (user.IsDeleted)
                return new Empty();
            
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Tokens.DeleteAllByUserIdAsync(user.Id);
                await _unitOfWork.Logins.DeleteAllByUserIdAsync(user.Id);
            
                user.Email = null;
                user.NewEmail = null;
                user.SecurityState!.PasswordHashed = null;
            
                _unitOfWork.Users.Delete(user);
            
                await _unitOfWork.SaveChangesAsync();
            
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
            
                _flexLogger.LogError(exception.Message, exception);
            
                throw new RpcException(new Status(StatusCode.Internal, "Something went wrong"));
            }
            
            return new Empty();
        }
    }
}
