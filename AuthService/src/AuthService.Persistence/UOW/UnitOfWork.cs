using Microsoft.EntityFrameworkCore.Storage;
using AuthService.Application.Abstractions.UOW;
using AuthService.Domain.Repositories.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.Repositories.UserManagement;

namespace AuthService.Persistence.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _authDbContext;

        public UnitOfWork(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => _authDbContext.Database.BeginTransactionAsync(cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
            => _authDbContext.Database.CommitTransactionAsync(cancellationToken);

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
            => _authDbContext.Database.RollbackTransactionAsync(cancellationToken);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _authDbContext.SaveChangesAsync(cancellationToken);

        private TInterface GetRepository<TInterface, TImplementation>(TInterface? repository, Func<AuthDbContext, TImplementation> ctor)
            where TImplementation : TInterface
        {
            if (repository == null)
            {
                repository = ctor(_authDbContext);
            }

            return repository;
        }

        private IUserRepository? users = null;
        public IUserRepository Users => GetRepository(users, (authDbContext) => new UserRepository(authDbContext));

        private IRoleRepository? roles = null;
        public IRoleRepository Roles => GetRepository(roles, (authDbContext) => new RoleRepository(authDbContext));

        private ILoginRepository? logins = null;
        public ILoginRepository Logins => GetRepository(logins, (authDbContext) => new LoginRepository(authDbContext));

        private ITokenRepository? tokens = null;
        public ITokenRepository Tokens => GetRepository(tokens, (authDbContext) => new TokenRepository(authDbContext));
    }
}
