namespace AdminService.Application.Dtos.Grpc.Client
{
    public class GrpcMetadataDto
    {
        public required Guid UserId { get; set; }
        public required string JwtBearerToken { get; set; }
        public required Guid CorrelationId { get; set; }
    }
}
