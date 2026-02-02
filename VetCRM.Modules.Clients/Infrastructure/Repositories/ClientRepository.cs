using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Infrastructure.Repositories
{
    public sealed class ClientRepository(ClientDbContext db) : IClientRepository
    {
        private readonly ClientDbContext _db = db;

        public async Task AddAsync(Client client, CancellationToken cancellationToken)
        {
            await _db.Clients.AddAsync(client, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _db.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public Task<bool> ExistsByPhoneAsync(string phone, Guid? excludeClientId, CancellationToken cancellationToken)
        {
            IQueryable<Client> query = _db.Clients.Where(c => c.Phone == phone);
            if (excludeClientId.HasValue)
                query = query.Where(c => c.Id != excludeClientId.Value);
            return query.AnyAsync(cancellationToken);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Client> Items, int TotalCount)> GetListAsync(
            string? search,
            int page,
            int pageSize,
            ClientStatus? status,
            CancellationToken cancellationToken)
        {
            IQueryable<Client> query = _db.Clients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(c =>
                    (c.FullName != null && c.FullName.ToLower().Contains(term)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(term)) ||
                    (c.Email != null && c.Email.ToLower().Contains(term)));
            }

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            int totalCount = await query.CountAsync(cancellationToken);

            IReadOnlyList<Client> items = await query
                .OrderBy(c => c.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
