using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Application.Queries
{
    public sealed class GetReminderLogHandler(IReminderLogRepository reminderLogRepository)
    {
        private readonly IReminderLogRepository _reminderLogRepository = reminderLogRepository;

        public async Task<GetReminderLogResult> Handle(GetReminderLogQuery query, CancellationToken ct)
        {
            var items = await _reminderLogRepository.GetFilteredAsync(
                query.Type,
                query.Status,
                query.From,
                query.To,
                ct);

            var dtos = items.Select(r => new ReminderLogDto(
                r.Id,
                r.Type.ToString(),
                r.TargetClientId,
                r.TargetPetId,
                r.Channel.ToString(),
                r.Payload,
                r.Status.ToString(),
                r.CreatedAt,
                r.Error)).ToList();

            return new GetReminderLogResult(dtos);
        }
    }
}
