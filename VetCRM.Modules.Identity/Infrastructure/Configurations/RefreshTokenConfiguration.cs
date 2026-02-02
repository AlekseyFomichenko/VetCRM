using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VetCRM.Modules.Identity.Infrastructure.Configurations
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(r => r.Token)
                .IsUnique();

            builder.Property(r => r.ExpiresAt)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();
        }
    }
}
