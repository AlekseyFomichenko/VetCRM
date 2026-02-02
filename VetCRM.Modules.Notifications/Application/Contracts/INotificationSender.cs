using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Application.Contracts
{
    public interface INotificationSender
    {
        ReminderChannel Channel { get; }
        Task SendAsync(string targetAddress, string payload, CancellationToken cancellationToken);
    }
}
