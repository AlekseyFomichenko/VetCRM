using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Notifications.Application.Commands;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Application.Queries;
using VetCRM.Modules.Notifications.Infrastructure;

namespace VetCRM.Modules.Notifications
{
    public static class Module
    {
        public static IServiceCollection AddNotificationsModule(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<NotificationsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.Configure<VaccinationReminderOptions>(options =>
            {
                options.ScheduledTimeUtc = new TimeOnly(8, 0);
            });

            services.AddScoped<IReminderLogRepository, ReminderLogRepository>();
            services.AddScoped<ProcessVaccinationRemindersHandler>();
            services.AddScoped<GetReminderLogHandler>();

            services.AddSingleton<INotificationSender, DemoNotificationSender>();
            services.AddSingleton<INotificationSender, EmailNotificationSenderStub>();
            services.AddSingleton<INotificationSender, SmsNotificationSenderStub>();

            services.AddHostedService<VaccinationReminderBackgroundService>();

            return services;
        }
    }
}
