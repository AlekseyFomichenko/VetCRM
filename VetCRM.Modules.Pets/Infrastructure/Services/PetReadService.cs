using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Pets.Application.Contracts;

namespace VetCRM.Modules.Pets.Infrastructure.Services
{
    public sealed class PetReadService(PetsDbContext db) : IPetReadService
    {
        private readonly PetsDbContext _db = db;

        public async Task<bool> ExistsAsync(Guid petId, CancellationToken ct)
        {
            return await _db.Pets.AnyAsync(p => p.Id == petId, ct);
        }

        public bool Exists(Guid petId)
        {
            return _db.Pets.Any(p => p.Id == petId);
        }

        public bool IsActive(Guid petId)
        {
            var pet = _db.Pets.AsNoTracking().FirstOrDefault(p => p.Id == petId);
            return pet?.Status == Domain.PetStatus.Active;
        }

        public Guid GetOwnerClientId(Guid petId)
        {
            var pet = _db.Pets.AsNoTracking().FirstOrDefault(p => p.Id == petId);
            return pet?.ClientId ?? Guid.Empty;
        }
    }
}