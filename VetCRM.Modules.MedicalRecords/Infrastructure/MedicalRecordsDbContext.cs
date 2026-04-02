using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Infrastructure
{
    public sealed class MedicalRecordsDbContext : DbContext
    {
        public MedicalRecordsDbContext()
        {
        }

        public MedicalRecordsDbContext(DbContextOptions<MedicalRecordsDbContext> options) : base(options)
        {
        }

        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Vaccination> Vaccinations => Set<Vaccination>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicalRecordsDbContext).Assembly);
        }
    }
}
