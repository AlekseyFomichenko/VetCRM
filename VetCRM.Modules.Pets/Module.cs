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

namespace VetCRM.Modules.Pets
{
    public static class Module
    {
        public static IServiceCollection AddPetsModule(this IServiceCollection services,
                                                       IConfiguration configuration)
        {
            services.AddDbContext<PetsDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IPetRepository, PetRepository>();

            services.AddScoped<CreatePetCommandHandler>();

            return services;
        }
    }
}
