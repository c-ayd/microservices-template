using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Repositories.UserManagement;
using AuthService.Persistence.DbContexts;

namespace AuthService.Persistence.Repositories.UserManagement
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _authDbContext;

        public RoleRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddAsync(Role newRole)
            => await _authDbContext.Roles.AddAsync(newRole);

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _authDbContext.Roles
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            => await _authDbContext.Roles
                .Where(r => r.Name == name)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<Role>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
            => await _authDbContext.Roles
                .AsNoTracking()
                .IgnoreQueryFilters()
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        public void Delete(Role role)
            => _authDbContext.Roles.Remove(role);
    }
}
