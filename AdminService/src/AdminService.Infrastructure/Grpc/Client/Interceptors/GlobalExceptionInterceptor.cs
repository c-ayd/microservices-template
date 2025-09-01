using AdminService.Application.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Net;

namespace AdminService.Infrastructure.Grpc.Client.Interceptors
{
    public class GlobalExceptionInterceptor : Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var call = continuation(request, context);
            return new AsyncUnaryCall<TResponse>(
                WrapResponse(call.ResponseAsync),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);
        }

        private async Task<TResponse> WrapResponse<TResponse>(Task<TResponse> responseAsync)
        {
            try
            {
                return await responseAsync;
            }
            catch (RpcException exception)
            {
                switch (exception.StatusCode)
                {
                    case StatusCode.InvalidArgument:
                    case StatusCode.OutOfRange:
                    case StatusCode.FailedPrecondition:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.BadRequest, exception, exception.Message);
                    case StatusCode.Unauthenticated:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.Unauthorized, exception, exception.Message);
                    case StatusCode.PermissionDenied:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.Forbidden, exception, exception.Message);
                    case StatusCode.NotFound:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.NotFound, exception, exception.Message);
                    case StatusCode.Cancelled:
                    case StatusCode.Aborted:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.RequestTimeout, exception, exception.Message);
                    case StatusCode.AlreadyExists:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.Conflict, exception, exception.Message);
                    case StatusCode.ResourceExhausted:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.TooManyRequests, exception, exception.Message);
                    case StatusCode.Unknown:
                    case StatusCode.Internal:
                    case StatusCode.DataLoss:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.InternalServerError, exception, exception.Message);
                    case StatusCode.Unimplemented:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.NotImplemented, exception, exception.Message);
                    case StatusCode.Unavailable:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.ServiceUnavailable, exception, exception.Message);
                    case StatusCode.DeadlineExceeded:
                        throw new GrpcException((int)exception.StatusCode, HttpStatusCode.GatewayTimeout, exception, exception.Message);
                    default:
                        throw;
                }
            }
        }
    }
}
