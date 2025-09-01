using AdminService.Application.Abstractions.Grpc.Client;
using AdminService.Application.Exceptions;
using AdminService.Application.Localization;
using Cayd.AspNetCore.ExecutionResult.ClientError;
using Cayd.AspNetCore.ExecutionResult.ServerError;
using Cayd.AspNetCore.FlexLog;
using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;

namespace AdminService.Application.MediatorFlows
{
    public class GrpcExceptionFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>, IGrpcRequest
    {
        private readonly IFlexLogger<GrpcExceptionFlow<TRequest, TResponse>> _flexLogger;

        public GrpcExceptionFlow(IFlexLogger<GrpcExceptionFlow<TRequest, TResponse>> flexLogger)
        {
            _flexLogger = flexLogger;
        }

        public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (GrpcException exception)
            {
                _flexLogger.LogError(exception.Message, exception);

                switch (exception.HttpStatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        return (dynamic)new ExecBadRequest("Invalid request parameters", GrpcLocalizationKeys.BadRequest);
                    case System.Net.HttpStatusCode.Unauthorized:
                        return (dynamic)new ExecUnauthorized("Unauthorized", GrpcLocalizationKeys.Unauthorized);
                    case System.Net.HttpStatusCode.Forbidden:
                        return (dynamic)new ExecForbidden("Access denied", GrpcLocalizationKeys.Forbidden);
                    case System.Net.HttpStatusCode.NotFound:
                        return (dynamic)new ExecNotFound("Resource is not found", GrpcLocalizationKeys.NotFound);
                    case System.Net.HttpStatusCode.RequestTimeout:
                        return (dynamic)new ExecRequestTimeout("Request timeout", GrpcLocalizationKeys.RequestTimeout);
                    case System.Net.HttpStatusCode.Conflict:
                        return (dynamic)new ExecConflict("Resource already exists", GrpcLocalizationKeys.Conflict);
                    case System.Net.HttpStatusCode.TooManyRequests:
                        return (dynamic)new ExecConflict("Too many requests", GrpcLocalizationKeys.TooManyRequest);
                    case System.Net.HttpStatusCode.InternalServerError:
                        return (dynamic)new ExecInternalServerError("Something went wrong", CommonLocalizationKeys.InternalServerError);
                    case System.Net.HttpStatusCode.NotImplemented:
                        return (dynamic)new ExecNotImplemented("Method is not implemented", GrpcLocalizationKeys.NotImplemented);
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        return (dynamic)new ExecServiceUnavailable("Unavailable", GrpcLocalizationKeys.ServiceUnavailable);
                    case System.Net.HttpStatusCode.GatewayTimeout:
                        return (dynamic)new ExecGetawayTimeout("Gateway timeout", GrpcLocalizationKeys.GatewayTimeout);
                    default:
                        throw;
                }
            }
        }
    }
}
