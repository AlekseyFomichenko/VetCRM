using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Contracts
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
        Task<(IReadOnlyList<User> Items, int TotalCount)> GetListAsync(
            string? search,
            UserRole? role,
            UserStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
