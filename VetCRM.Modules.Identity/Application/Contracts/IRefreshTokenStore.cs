namespace VetCRM.Modules.Identity.Application.Contracts
{
    public interface IRefreshTokenStore
    {
        Task AddAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken);
        Task<Guid?> GetUserIdByTokenAsync(string token, CancellationToken cancellationToken);
        Task RevokeAsync(string token, CancellationToken cancellationToken);
    }
}
