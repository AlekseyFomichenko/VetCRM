using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Application.Contracts
{
    public interface IClientRepository
    {
        Task AddAsync(Client client, CancellationToken cancellationToken);
        Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByPhoneAsync(string phone, Guid? excludeClientId, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
        Task<(IReadOnlyList<Client> Items, int TotalCount)> GetListAsync(
            string? search,
            int page,
            int pageSize,
            ClientStatus? status,
            CancellationToken cancellationToken);
    }
}
