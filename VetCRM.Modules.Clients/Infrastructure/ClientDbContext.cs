using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Clients.Infrastructure.Persistence;

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

        public DbSet<ClientReadModel> Clients => Set<ClientReadModel>();
    }
}
