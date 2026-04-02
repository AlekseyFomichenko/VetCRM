using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.Modules.Pets.Application.Contracts
{
    public interface IPetReadService
    {
        Task<bool> ExistsAsync(Guid petId, CancellationToken ct);
        bool Exists(Guid petId);
        bool IsActive(Guid petId);
        Guid GetOwnerClientId(Guid petId);
    }
}
