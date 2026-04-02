using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VetCRM.Modules.Notifications.Application.Commands;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class VaccinationReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<VaccinationReminderBackgroundService> logger,
        IOptions<VaccinationReminderOptions> options) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<VaccinationReminderBackgroundService> _logger = logger;
        private readonly VaccinationReminderOptions _options = options.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var runAt = now.Date.Add(_options.ScheduledTimeUtc.ToTimeSpan());
                if (runAt <= now)
                    runAt = runAt.AddDays(1);

                var delay = runAt - now;
                _logger.LogInformation("Next vaccination reminder run at {RunAt}", runAt);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<ProcessVaccinationRemindersHandler>();
                    var result = await handler.Handle(new ProcessVaccinationRemindersCommand(), stoppingToken);
                    _logger.LogInformation(
                        "Vaccination reminders: created={Created}, sent={Sent}, failed={Failed}",
                        result.Created,
                        result.Sent,
                        result.Failed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Vaccination reminder job failed");
                }
            }
        }
    }

    public sealed class VaccinationReminderOptions
    {
        public TimeOnly ScheduledTimeUtc { get; set; } = new(8, 0);
    }
}
