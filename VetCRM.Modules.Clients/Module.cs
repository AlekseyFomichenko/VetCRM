using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Infrastructure;
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

            return services;
        }
    }
}
