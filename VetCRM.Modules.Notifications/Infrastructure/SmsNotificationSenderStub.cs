using Microsoft.Extensions.Logging;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class SmsNotificationSenderStub(ILogger<SmsNotificationSenderStub> logger) : INotificationSender
    {
        private readonly ILogger<SmsNotificationSenderStub> _logger = logger;

        public ReminderChannel Channel => ReminderChannel.Sms;

        public Task SendAsync(string targetAddress, string payload, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SMS stub: would send to {Phone}: {Payload}", targetAddress, payload);
            return Task.CompletedTask;
        }
    }
}
