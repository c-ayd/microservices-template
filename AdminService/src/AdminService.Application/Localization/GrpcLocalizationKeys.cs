namespace AdminService.Application.Localization
{
    public static class GrpcLocalizationKeys
    {
        /** 
         * NOTE: These values are only placeholders. If your frontend is multilingual,
         * you can return localization codes from the backend to show corresponding
         * localized messages to end users.
         * 
         * If this class gets bigger over time, consider using partial classes to divide
         * const values into different code files for grouping and maintability.
         */

        public const string BadRequest = "grpc_bad_request";
        public const string Unauthorized = "grpc_unauthorized";
        public const string Forbidden = "grpc_forbidden";
        public const string NotFound = "grpc_not_found";
        public const string RequestTimeout = "grpc_request_timeout";
        public const string Conflict = "grpc_conflict";
        public const string TooManyRequest = "grpc_too_many_requests";
        public const string NotImplemented = "grpc_not_implemented";
        public const string ServiceUnavailable = "grpc_service_unavailable";
        public const string GatewayTimeout = "grpc_gateway_timeout";
    }
}
