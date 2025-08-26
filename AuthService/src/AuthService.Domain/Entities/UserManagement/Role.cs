using AuthService.Domain.SeedWork;

namespace AuthService.Domain.Entities.UserManagement
{
    public class Role : EntityBase<int>, ISoftDelete
    {
        public string Name { get; set; } = null!;

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedDate { get; private set; }

        // Relationships
        public List<User> Users { get; set; } = new List<User>();
    }
}
