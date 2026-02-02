using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VetCRM.Modules.Identity.Infrastructure.Configurations
{
    public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetTokenEntity>
    {
        public void Configure(EntityTypeBuilder<PasswordResetTokenEntity> builder)
        {
            builder.ToTable("PasswordResetTokens");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Token)
                .IsUnique();

            builder.Property(p => p.ExpiresAt)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
