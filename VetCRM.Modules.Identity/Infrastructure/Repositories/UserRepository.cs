using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Infrastructure.Repositories
{
    public sealed class UserRepository(IdentityDbContext db) : IUserRepository
    {
        private readonly IdentityDbContext _db = db;

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            string normalized = email.Trim().ToLowerInvariant();
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId, CancellationToken cancellationToken)
        {
            string normalized = email.Trim().ToLowerInvariant();
            IQueryable<User> query = _db.Users.Where(u => u.Email == normalized);
            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);
            return await query.AnyAsync(cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetListAsync(
            string? search,
            UserRole? role,
            UserStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            IQueryable<User> query = _db.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLowerInvariant();
                query = query.Where(u =>
                    u.Email.Contains(term) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(term)));
            }

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            int totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
