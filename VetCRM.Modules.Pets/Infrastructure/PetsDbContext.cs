using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Infrastructure
{
    public sealed class PetsDbContext : DbContext
    {
        public PetsDbContext()
        {
        }

        public PetsDbContext(DbContextOptions<PetsDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets => Set<Pet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PetsDbContext).Assembly);
        }
    }
}
