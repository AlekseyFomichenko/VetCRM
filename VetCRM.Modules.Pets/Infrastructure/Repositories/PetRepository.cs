using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Infrastructure.Repositories
{
    public sealed class PetRepository(PetsDbContext db) : IPetRepository
    {
        private readonly PetsDbContext _db = db;
        public async Task AddAsync(Pet pet, CancellationToken cancellationToken)
        {
            await _db.AddAsync(pet, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
