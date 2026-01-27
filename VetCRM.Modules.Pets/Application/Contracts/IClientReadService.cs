using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.Modules.Pets.Application.Contracts
{
    public interface IClientReadService
    {
        Task<bool> ExistsAsync(Guid clientId, CancellationToken cancellationToken);
    }
}
