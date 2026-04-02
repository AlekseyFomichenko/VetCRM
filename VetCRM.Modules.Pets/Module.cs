using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Pets.Application.Commands;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Pets.Application.Queries;
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
                options.UseNpgsql(connectionString));

            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IPetReadService, PetReadService>();
            services.AddScoped<CreatePetHandler>();
            services.AddScoped<GetPetByIdHandler>();
            services.AddScoped<GetPetsHandler>();
            services.AddScoped<UpdatePetHandler>();
            services.AddScoped<ArchivePetHandler>();
            services.AddScoped<SetPetClientHandler>();

            return services;
        }
    }
}
