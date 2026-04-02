using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Modules.Notifications.Infrastructure
{
    public sealed class NotificationsDbContext : DbContext
    {
        public NotificationsDbContext()
        {
        }

        public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
        {
        }

        public DbSet<ReminderLog> ReminderLogs => Set<ReminderLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        }
    }
}
