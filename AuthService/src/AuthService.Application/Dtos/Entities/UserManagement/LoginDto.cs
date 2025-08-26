using System.Net;

namespace AuthService.Application.Dtos.Entities.UserManagement
{
    public class LoginDto
    {
        public Guid Id { get; set; }

        public IPAddress? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UserId { get; set; }
    }
}
