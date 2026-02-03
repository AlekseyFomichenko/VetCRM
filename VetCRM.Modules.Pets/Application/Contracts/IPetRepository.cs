using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Application.Contracts
{
    public interface IPetRepository
    {
        Task AddAsync(Pet pet, CancellationToken cancellationToken);
        Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
        Task<(IReadOnlyList<Pet> Items, int TotalCount)> GetListAsync(
            string? search,
            int page,
            int pageSize,
            Guid? clientId,
            PetStatus? status,
            CancellationToken cancellationToken);
    }
}
