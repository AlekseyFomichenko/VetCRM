using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Application.Contracts
{
    public interface IReminderLogRepository
    {
        Task AddAsync(ReminderLog reminderLog, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<ReminderLog>> GetFilteredAsync(
            ReminderType? type,
            ReminderStatus? status,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken);
    }
}
