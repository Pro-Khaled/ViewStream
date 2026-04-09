using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;
using ViewStream.Infrastructure.Repositories;
using ViewStream.Infrastructure.Services;
using ViewStream.Infrastructure.UnitOfWorks;
using ViewStream.Shared.Options;




namespace ViewStream.Infrastructure
{
    /// <summary>
    /// Dependency Injection configuration for Infrastructure layer
    /// </summary>
    public static class AddInfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ViewStreamDbContext>((serviceProvider, options) =>
            {
                var connectionOptions = serviceProvider
                   .GetRequiredService<IOptions<DatabaseConnectionOptions>>()
                   .Value;

                var dbOptions = serviceProvider
                    .GetRequiredService<IOptions<DatabaseOptions>>()
                    .Value;

                options.UseSqlServer(connectionOptions.DefaultConnection, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(dbOptions.CommandTimeout);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: dbOptions.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(dbOptions.MaxRetryDelay),
                        errorNumbersToAdd: null);
                });

                if (dbOptions.EnableDetailedErrors)
                {
                    options.EnableDetailedErrors();
                }

                if (dbOptions.EnableSensitiveDataLogging)
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            // Register Identity Service
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<ViewStreamDbContext>().AddDefaultTokenProviders(); ;

            // Register Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register JWT Token Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register Email Service
            services.AddScoped<IEmailService, EmailService>();
            
            return services;
        }
    }
}
