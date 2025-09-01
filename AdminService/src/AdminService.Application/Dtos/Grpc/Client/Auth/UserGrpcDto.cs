namespace AdminService.Application.Dtos.Grpc.Client.Auth
{
    public class UserGrpcDto
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
