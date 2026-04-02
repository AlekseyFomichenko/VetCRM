using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Reports.Application.Queries;

namespace VetCRM.Modules.Reports
{
    public static class Module
    {
        public static IServiceCollection AddReportsModule(this IServiceCollection services)
        {
            services.AddScoped<GetAppointmentsReportHandler>();
            services.AddScoped<GetOverdueVaccinationsReportHandler>();

            return services;
        }
    }
}
