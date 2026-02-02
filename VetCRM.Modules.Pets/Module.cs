using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Pets.Application.Commands;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Pets.Infrastructure;
using VetCRM.Modules.Pets.Infrastructure.Repositories;
using VetCRM.Modules.Pets.Infrastructure.Services;

namespace VetCRM.Modules.Pets
{
    public static class Module
    {
        public static IServiceCollection AddPetsModule(this IServiceCollection services,
                                                       string connectionString)
        {
            services.AddDbContext<PetsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IPetReadService, PetReadService>();
            services.AddScoped<CreatePetHandler>();

            return services;
        }
    }
}
