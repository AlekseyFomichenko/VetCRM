using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Infrastructure
{
    public sealed class ClientDbContext : DbContext
    {
        public ClientDbContext()
        {
        }

        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientDbContext).Assembly);
        }
    }
}
