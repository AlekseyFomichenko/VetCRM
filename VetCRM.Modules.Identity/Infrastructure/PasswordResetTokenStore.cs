using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Identity.Application.Contracts;

namespace VetCRM.Modules.Identity.Infrastructure
{
    public sealed class PasswordResetTokenStore(IdentityDbContext db) : IPasswordResetTokenStore
    {
        private readonly IdentityDbContext _db = db;

        public async Task AddAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var entity = new PasswordResetTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };
            await _db.PasswordResetTokens.AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<Guid?> GetUserIdByTokenAsync(string token, CancellationToken cancellationToken)
        {
            var entity = await _db.PasswordResetTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Token == token && p.ExpiresAt > DateTime.UtcNow, cancellationToken);
            return entity?.UserId;
        }

        public async Task ConsumeAsync(string token, CancellationToken cancellationToken)
        {
            var entity = await _db.PasswordResetTokens.FirstOrDefaultAsync(p => p.Token == token, cancellationToken);
            if (entity is not null)
            {
                _db.PasswordResetTokens.Remove(entity);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
