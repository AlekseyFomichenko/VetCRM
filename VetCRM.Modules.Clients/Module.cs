using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Clients.Application.Commands;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Application.Queries;
using VetCRM.Modules.Clients.Infrastructure;
using VetCRM.Modules.Clients.Infrastructure.Repositories;
using VetCRM.Modules.Clients.Infrastructure.Services;

namespace VetCRM.Modules.Clients
{
    public static class Module
    {
        public static IServiceCollection AddClientModule(this IServiceCollection services,
                                                         string connectionString)
        {
            services.AddDbContext<ClientDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IClientReadService, ClientReadService>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<CreateClientHandler>();
            services.AddScoped<UpdateClientHandler>();
            services.AddScoped<ArchiveClientHandler>();
            services.AddScoped<GetClientByIdHandler>();
            services.AddScoped<GetClientsHandler>();

            return services;
        }
    }
}
