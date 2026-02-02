using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure.Configurations
{
    public sealed class ReminderLogConfiguration : IEntityTypeConfiguration<ReminderLog>
    {
        public void Configure(EntityTypeBuilder<ReminderLog> builder)
        {
            builder.ToTable("ReminderLogs");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Type).IsRequired();
            builder.Property(r => r.TargetClientId);
            builder.Property(r => r.TargetPetId);
            builder.Property(r => r.Channel).IsRequired();
            builder.Property(r => r.Payload).IsRequired().HasMaxLength(4000);
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.Error).HasMaxLength(1000);
        }
    }
}
