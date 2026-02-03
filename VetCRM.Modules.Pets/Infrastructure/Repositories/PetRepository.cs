using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Infrastructure.Repositories
{
    public sealed class PetRepository(PetsDbContext db) : IPetRepository
    {
        private readonly PetsDbContext _db = db;

        public async Task AddAsync(Pet pet, CancellationToken cancellationToken)
        {
            await _db.Pets.AddAsync(pet, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            _db.Pets.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public Task SaveAsync(CancellationToken cancellationToken) =>
            _db.SaveChangesAsync(cancellationToken);

        public async Task<(IReadOnlyList<Pet> Items, int TotalCount)> GetListAsync(
            string? search,
            int page,
            int pageSize,
            Guid? clientId,
            PetStatus? status,
            CancellationToken cancellationToken)
        {
            IQueryable<Pet> query = _db.Pets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(term)) ||
                    (p.Species != null && p.Species.ToLower().Contains(term)));
            }

            if (clientId.HasValue)
                query = query.Where(p => p.ClientId == clientId.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            int totalCount = await query.CountAsync(cancellationToken);

            IReadOnlyList<Pet> items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
