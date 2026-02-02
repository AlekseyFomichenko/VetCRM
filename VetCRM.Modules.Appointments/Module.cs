using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Appointments.Application.Commands;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Application.Queries;
using VetCRM.Modules.Appointments.Infrastructure;
using VetCRM.Modules.Appointments.Infrastructure.Repositories;

namespace VetCRM.Modules.Appointments
{
    public static class Module
    {
        public static IServiceCollection AddAppointmentsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppointmentsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentsForReportQuery, AppointmentsForReportQuery>();
            services.AddScoped<CreateAppointmentHandler>();
            services.AddScoped<RescheduleAppointmentHandler>();
            services.AddScoped<CancelAppointmentHandler>();
            services.AddScoped<CompleteAppointmentHandler>();
            services.AddScoped<GetAppointmentsByDateHandler>();

            return services;
        }
    }
}
