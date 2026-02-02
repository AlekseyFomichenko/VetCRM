namespace VetCRM.Modules.Identity.Application.Contracts
{
    public interface IPasswordResetTokenStore
    {
        Task AddAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken);
        Task<Guid?> GetUserIdByTokenAsync(string token, CancellationToken cancellationToken);
        Task ConsumeAsync(string token, CancellationToken cancellationToken);
    }
}
