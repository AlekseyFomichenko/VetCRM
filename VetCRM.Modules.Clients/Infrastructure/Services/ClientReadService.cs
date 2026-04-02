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

        public async Task<ClientContactDto?> GetContactAsync(Guid clientId, CancellationToken ct)
        {
            var client = await _dbContext.Clients
                .AsNoTracking()
                .Where(c => c.Id == clientId)
                .Select(c => new { c.FullName, c.Phone, c.Email })
                .FirstOrDefaultAsync(ct);
            return client is null ? null : new ClientContactDto(client.FullName, client.Phone, client.Email);
        }
    }
}
