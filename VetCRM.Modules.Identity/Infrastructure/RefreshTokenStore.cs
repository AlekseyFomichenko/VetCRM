using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Identity.Application.Contracts;

namespace VetCRM.Modules.Identity.Infrastructure
{
    public sealed class RefreshTokenStore(IdentityDbContext db) : IRefreshTokenStore
    {
        private readonly IdentityDbContext _db = db;

        public async Task AddAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var entity = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };
            await _db.RefreshTokens.AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<Guid?> GetUserIdByTokenAsync(string token, CancellationToken cancellationToken)
        {
            var entity = await _db.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Token == token && r.ExpiresAt > DateTime.UtcNow, cancellationToken);
            return entity?.UserId;
        }

        public async Task RevokeAsync(string token, CancellationToken cancellationToken)
        {
            var entity = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
            if (entity is not null)
            {
                _db.RefreshTokens.Remove(entity);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
