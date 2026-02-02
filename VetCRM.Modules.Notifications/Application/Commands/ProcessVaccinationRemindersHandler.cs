using System.Text.Json;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;
using VetCRM.Modules.Pets.Application.Contracts;

namespace VetCRM.Modules.Notifications.Application.Commands
{
    public sealed class ProcessVaccinationRemindersHandler(
        IUpcomingVaccinationsQuery upcomingVaccinationsQuery,
        IPetReadService petReadService,
        IClientReadService clientReadService,
        IReminderLogRepository reminderLogRepository,
        IEnumerable<INotificationSender> senders)
    {
        private const int ReminderDays = 3;

        private readonly IUpcomingVaccinationsQuery _upcomingVaccinationsQuery = upcomingVaccinationsQuery;
        private readonly IPetReadService _petReadService = petReadService;
        private readonly IClientReadService _clientReadService = clientReadService;
        private readonly IReminderLogRepository _reminderLogRepository = reminderLogRepository;
        private readonly IReadOnlyList<INotificationSender> _senders = senders.ToList();

        public async Task<ProcessVaccinationRemindersResult> Handle(
            ProcessVaccinationRemindersCommand command,
            CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var upToDateExclusive = today.AddDays(ReminderDays + 1);

            var upcoming = await _upcomingVaccinationsQuery.GetUpcomingAsync(today, upToDateExclusive, ct);

            int created = 0;
            int sent = 0;
            int failed = 0;

            foreach (var v in upcoming)
            {
                Guid clientId = _petReadService.GetOwnerClientId(v.PetId);
                if (clientId == Guid.Empty)
                    continue;

                var contact = await _clientReadService.GetContactAsync(clientId, ct);
                if (contact is null)
                    continue;

                ReminderChannel channel = ReminderChannel.Demo;
                string targetAddress = "demo";
                if (!string.IsNullOrWhiteSpace(contact.Email))
                {
                    channel = ReminderChannel.Email;
                    targetAddress = contact.Email;
                }
                else if (!string.IsNullOrWhiteSpace(contact.Phone))
                {
                    channel = ReminderChannel.Sms;
                    targetAddress = contact.Phone;
                }

                var payload = JsonSerializer.Serialize(new
                {
                    v.VaccinationId,
                    v.PetId,
                    v.VaccineName,
                    v.NextDueDate
                });

                var reminder = ReminderLog.Create(
                    ReminderType.VaccinationDue,
                    clientId,
                    v.PetId,
                    channel,
                    payload);

                created++;

                INotificationSender? sender = _senders.FirstOrDefault(s => s.Channel == channel);
                if (sender is null)
                    sender = _senders.FirstOrDefault(s => s.Channel == ReminderChannel.Demo);

                try
                {
                    await (sender?.SendAsync(targetAddress, payload, ct) ?? Task.CompletedTask);
                    sent++;
                }
                catch (Exception ex)
                {
                    reminder.MarkFailed(ex.Message);
                    failed++;
                }

                await _reminderLogRepository.AddAsync(reminder, ct);
            }

            await _reminderLogRepository.SaveAsync(ct);

            return new ProcessVaccinationRemindersResult(created, sent, failed);
        }
    }
}
