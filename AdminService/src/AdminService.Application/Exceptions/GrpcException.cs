using System.Net;

namespace AdminService.Application.Exceptions
{
    public class GrpcException : Exception
    {
        public int StatusCode { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public Exception Exception { get; private set; }

        public GrpcException(int statusCode, HttpStatusCode httpStatusCode, Exception exception, string? message = null)
            : base(message)
        {
            StatusCode = statusCode;
            HttpStatusCode = httpStatusCode;
            Exception = exception;
        }
    }
}
