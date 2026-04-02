using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Infrastructure.Configurations
{
    public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.PetId).IsRequired();
            builder.Property(a => a.ClientId).IsRequired();
            builder.Property(a => a.VeterinarianUserId);
            builder.Property(a => a.StartsAt).IsRequired();
            builder.Property(a => a.EndsAt).IsRequired();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.Reason).HasMaxLength(500);
            builder.Property(a => a.CreatedByUserId);
            builder.Property(a => a.CreatedAt).IsRequired();

            builder.HasIndex(a => new { a.VeterinarianUserId, a.StartsAt });
        }
    }
}
