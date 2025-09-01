using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Abstractions.Http;
using AdminService.Application.Dtos.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.FlexLog;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Commands.DeleteUser
{
    public class DeleteUserHandler : IAsyncHandler<DeleteUserRequest, ExecResult<DeleteUserResponse>>
    {
        private readonly IAuthGrpcClientService _authGrpcClient;
        private readonly IRequestContext _requestContext;
        private readonly IFlexLogger<DeleteUserHandler> _flexLogger;

        public DeleteUserHandler(IAuthGrpcClientService authGrpcClient,
            IRequestContext requestContext,
            IFlexLogger<DeleteUserHandler> flexLogger)
        {
            _authGrpcClient = authGrpcClient;
            _requestContext = requestContext;
            _flexLogger = flexLogger;
        }

        public async Task<ExecResult<DeleteUserResponse>> HandleAsync(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var metadata = new GrpcMetadataDto()
            {
                UserId = _requestContext.UserId!.Value,
                JwtBearerToken = _requestContext.JwtBearerToken!,
                CorrelationId = _flexLogger.LogContext.CorrelationId
            };

            return await _authGrpcClient.DeleteUserAsync(request, metadata);
        }
    }
}
