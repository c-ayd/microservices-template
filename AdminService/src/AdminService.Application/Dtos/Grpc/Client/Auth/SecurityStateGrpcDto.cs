namespace AdminService.Application.Dtos.Grpc.Client.Auth
{
    public class SecurityStateGrpcDto
    {
        public bool IsEmailVerified { get; set; }
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? UnlockDate { get; set; }
    }
}
