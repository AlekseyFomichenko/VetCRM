using Microsoft.Extensions.Logging;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class EmailNotificationSenderStub(ILogger<EmailNotificationSenderStub> logger) : INotificationSender
    {
        private readonly ILogger<EmailNotificationSenderStub> _logger = logger;

        public ReminderChannel Channel => ReminderChannel.Email;

        public Task SendAsync(string targetAddress, string payload, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email stub: would send to {Email}: {Payload}", targetAddress, payload);
            return Task.CompletedTask;
        }
    }
}
