using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.MedicalRecords.Application.Commands;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Application.Queries;
using VetCRM.Modules.MedicalRecords.Infrastructure;
using VetCRM.Modules.MedicalRecords.Infrastructure.Repositories;

namespace VetCRM.Modules.MedicalRecords
{
    public static class Module
    {
        public static IServiceCollection AddMedicalRecordsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MedicalRecordsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IVaccinationRepository, VaccinationRepository>();
            services.AddScoped<IUpcomingVaccinationsQuery, UpcomingVaccinationsQuery>();
            services.AddScoped<ICreateMedicalRecordFromAppointment, CreateMedicalRecordFromAppointmentHandler>();
            services.AddScoped<CreateMedicalRecordHandler>();
            services.AddScoped<UpdateMedicalRecordHandler>();
            services.AddScoped<AddVaccinationHandler>();
            services.AddScoped<GetMedicalRecordByIdHandler>();
            services.AddScoped<GetMedicalRecordsByPetIdHandler>();

            return services;
        }
    }
}
