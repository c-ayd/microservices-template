using System.Net;
using AuthService.Domain.SeedWork;

namespace AuthService.Domain.Entities.UserManagement
{
    public class Login : EntityBase<Guid>, IUpdateAudit
    {
        public string RefreshTokenHashed { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        public IPAddress? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }

        public DateTime? UpdatedDate { get; private set; }
        
        // Relationships
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
