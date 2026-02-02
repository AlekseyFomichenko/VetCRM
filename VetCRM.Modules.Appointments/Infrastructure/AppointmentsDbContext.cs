using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Infrastructure
{
    public sealed class AppointmentsDbContext : DbContext
    {
        public AppointmentsDbContext()
        {
        }

        public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentsDbContext).Assembly);
        }
    }
}
