using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Abstractions.Http;
using AdminService.Application.Dtos.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.FlexLog;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Queries.GetUser
{
    public class GetUserHandler : IAsyncHandler<GetUserRequest, ExecResult<GetUserResponse>>
    {
        private readonly IAuthGrpcClientService _authGrpcClient;
        private readonly IRequestContext _requestContext;
        private readonly IFlexLogger<GetUserHandler> _flexLogger;

        public GetUserHandler(IAuthGrpcClientService authGrpcClient,
            IRequestContext requestContext,
            IFlexLogger<GetUserHandler> flexLogger)
        {
            _authGrpcClient = authGrpcClient;
            _requestContext = requestContext;
            _flexLogger = flexLogger;
        }

        public async Task<ExecResult<GetUserResponse>> HandleAsync(GetUserRequest request, CancellationToken cancellationToken)
        {
            var metadata = new GrpcMetadataDto()
            {
                UserId = _requestContext.UserId!.Value,
                JwtBearerToken = _requestContext.JwtBearerToken!,
                CorrelationId = _flexLogger.LogContext.CorrelationId
            };

            return await _authGrpcClient.GetUserAsync(request, metadata, cancellationToken);
        }
    }
}
