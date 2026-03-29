using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VetCRM.Modules.Identity.Application.Commands;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Application.Queries;
using VetCRM.Modules.Identity.Infrastructure;
using VetCRM.Modules.Identity.Infrastructure.Repositories;

namespace VetCRM.Modules.Identity
{
    public static class Module
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
            services.AddScoped<IPasswordResetTokenStore, PasswordResetTokenStore>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IEmailSender, StubEmailSender>();

            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<LoginHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<ForgotPasswordHandler>();
            services.AddScoped<ResetPasswordHandler>();
            services.AddScoped<CreateUserHandler>();
            services.AddScoped<UpdateUserHandler>();
            services.AddScoped<DisableUserHandler>();

            services.AddScoped<GetUserByIdHandler>();
            services.AddScoped<GetUsersHandler>();
            services.AddScoped<GetVeterinariansForSchedulingHandler>();

            return services;
        }
    }
}
