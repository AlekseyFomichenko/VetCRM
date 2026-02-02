using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Infrastructure.Configurations
{
    public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.Phone)
                .IsUnique();

            builder.Property(c => c.Email)
                .HasMaxLength(200);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.Notes)
                .HasMaxLength(2000);

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();
        }
    }
}
