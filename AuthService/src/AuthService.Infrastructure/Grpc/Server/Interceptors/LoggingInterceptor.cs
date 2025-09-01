using Cayd.AspNetCore.FlexLog;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Globalization;

namespace AuthService.Infrastructure.Grpc.Server.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly IFlexLogger<LoggingInterceptor> _flexLogger;

        public LoggingInterceptor(IFlexLogger<LoggingInterceptor> flexLogger)
        {
            _flexLogger = flexLogger;
        }

        private void AddLogDetails(Metadata metadata)
        {
            _flexLogger.LogContext.Timestamp = DateTime.Parse(metadata.GetValue(GrpcHeaderKeys.Timestamp)!, null, DateTimeStyles.RoundtripKind);

            var userId = metadata.GetValue(GrpcHeaderKeys.UserId);
            if (userId != null)
            {
                _flexLogger.LogInformation($"The call is made by {metadata.GetValue(GrpcHeaderKeys.UserId)}");
            }
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                AddLogDetails(context.RequestHeaders);

                return await continuation(request, context);
            }
            catch (Exception exception)
            {
                _flexLogger.LogError(exception.Message, exception);

                throw;
            }
            finally
            {
                _flexLogger.LogContext.ResponseStatusCode = (int)context.Status.StatusCode;
            }
        }
    }
}
