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
    }
}
