using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Abstractions.Http;
using AdminService.Application.Dtos.Grpc.Client;
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.FlexLog;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace AdminService.Application.Features.Queries.GetUsers
{
    public class GetUsersHandler : IAsyncHandler<GetUsersRequest, ExecResult<GetUsersResponse>>
    {
        private readonly IAuthGrpcClientService _authGrpcClient;
        private readonly IRequestContext _requestContext;
        private readonly IFlexLogger<GetUsersHandler> _flexLogger;

        public GetUsersHandler(IAuthGrpcClientService authGrpcClient,
            IRequestContext requestContext,
            IFlexLogger<GetUsersHandler> flexLogger)
        {
            _authGrpcClient = authGrpcClient;
            _requestContext = requestContext;
            _flexLogger = flexLogger;
        }

        public async Task<ExecResult<GetUsersResponse>> HandleAsync(GetUsersRequest request, CancellationToken cancellationToken)
        {
            var metadata = new GrpcMetadataDto()
            {
                UserId = _requestContext.UserId!.Value,
                JwtBearerToken = _requestContext.JwtBearerToken!,
                CorrelationId = _flexLogger.LogContext.CorrelationId
            };

            return await _authGrpcClient.GetUsersAsync(request, metadata, cancellationToken);
        }
    }
}
