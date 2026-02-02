using Microsoft.Extensions.Logging;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class DemoNotificationSender(ILogger<DemoNotificationSender> logger) : INotificationSender
    {
        private readonly ILogger<DemoNotificationSender> _logger = logger;

        public ReminderChannel Channel => ReminderChannel.Demo;

        public Task SendAsync(string targetAddress, string payload, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Demo notification to {Target}: {Payload}", targetAddress, payload);
            return Task.CompletedTask;
        }
    }
}
