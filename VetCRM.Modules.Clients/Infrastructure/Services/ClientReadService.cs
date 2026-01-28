using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Clients.Application.Contracts;

namespace VetCRM.Modules.Clients.Infrastructure.Services
{
    public sealed class ClientReadService(ClientDbContext dbClient) : IClientReadService
    {
        private readonly ClientDbContext _dbContext = dbClient;

        public async Task<bool> ExistsAsync(Guid clientId, CancellationToken ct)
        {
            return await _dbContext.Clients.AnyAsync(c => c.Id == clientId, ct);
        }
    }
}
