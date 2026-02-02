namespace VetCRM.Modules.Clients.Application.Contracts
{
    public interface IClientReadService
    {
        Task<bool> ExistsAsync(Guid clientId, CancellationToken ct);
        Task<ClientContactDto?> GetContactAsync(Guid clientId, CancellationToken ct);
    }
}
