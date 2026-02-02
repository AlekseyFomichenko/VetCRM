using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class ReminderLogRepository(NotificationsDbContext db) : IReminderLogRepository
    {
        private readonly NotificationsDbContext _db = db;

        public async Task AddAsync(ReminderLog reminderLog, CancellationToken cancellationToken)
        {
            await _db.ReminderLogs.AddAsync(reminderLog, cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ReminderLog>> GetFilteredAsync(
            ReminderType? type,
            ReminderStatus? status,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken)
        {
            IQueryable<ReminderLog> query = _db.ReminderLogs.AsNoTracking();

            if (type.HasValue)
                query = query.Where(r => r.Type == type.Value);

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (from.HasValue)
                query = query.Where(r => r.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(r => r.CreatedAt <= to.Value);

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
