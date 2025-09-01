namespace AdminService.Application.Dtos.Grpc.Client.Auth
{
    public class LoginGrpcDto
    {
        public string? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
