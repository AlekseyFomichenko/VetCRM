using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Application.Queries
{
    public sealed record GetReminderLogQuery(
        ReminderType? Type,
        ReminderStatus? Status,
        DateTime? From,
        DateTime? To);
}
