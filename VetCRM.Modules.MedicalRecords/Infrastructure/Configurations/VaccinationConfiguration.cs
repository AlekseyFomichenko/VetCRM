using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Infrastructure.Configurations
{
    public sealed class VaccinationConfiguration : IEntityTypeConfiguration<Vaccination>
    {
        public void Configure(EntityTypeBuilder<Vaccination> builder)
        {
            builder.ToTable("Vaccinations");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.MedicalRecordId).IsRequired();
            builder.Property(v => v.VaccineName).IsRequired().HasMaxLength(200);
            builder.Property(v => v.VaccinationDate).IsRequired();
            builder.Property(v => v.NextDueDate);
            builder.Property(v => v.Batch).HasMaxLength(100);
            builder.Property(v => v.Manufacturer).HasMaxLength(200);
        }
    }
}
