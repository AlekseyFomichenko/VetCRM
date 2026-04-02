using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Infrastructure
{
    public sealed class IdentityDbContext : DbContext
    {
        public IdentityDbContext()
        {
        }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
        public DbSet<PasswordResetTokenEntity> PasswordResetTokens => Set<PasswordResetTokenEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        }
    }
}
