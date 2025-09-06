using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement.Enums;
using AuthService.Domain.Repositories.UserManagement;
using AuthService.Persistence.DbContexts;

namespace AuthService.Persistence.Repositories.UserManagement
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AuthDbContext _authDbContext;

        public TokenRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddAsync(Token newToken)
            => await _authDbContext.Tokens.AddAsync(newToken);

        public async Task<Token?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.Id.Equals(id))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<Token?> GetByHashedValueAndPurposeAsync(string hashedValue, ETokenPurpose purpose, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.ValueHashed == hashedValue &&
                    t.Purpose == purpose)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<Token>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.UserId.Equals(userId))
                .ToListAsync(cancellationToken);

        public async Task<List<Token>> GetAllByUserIdAndPurposeAsync(Guid userId, ETokenPurpose purpose, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.UserId.Equals(userId) &&
                    t.Purpose == purpose)
                .ToListAsync(cancellationToken);

        public void Delete(Token token)
            => _authDbContext.Tokens.Remove(token);

        public void DeleteAll(IEnumerable<Token> tokens)
            => _authDbContext.Tokens.RemoveRange(tokens);

        public async Task<int> DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.UserId.Equals(userId))
                .ExecuteDeleteAsync(cancellationToken);

        public async Task<int> DeleteAllByUserIdAndPurposeAsync(Guid userId, ETokenPurpose purpose, CancellationToken cancellationToken = default)
            => await _authDbContext.Tokens
                .Where(t => t.UserId.Equals(userId) &&
                    t.Purpose == purpose)
                .ExecuteDeleteAsync(cancellationToken);
    }
}
