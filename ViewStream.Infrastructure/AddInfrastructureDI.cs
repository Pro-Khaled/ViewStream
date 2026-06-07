using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.MessageBus;
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

            // JWT Options
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            // Email Options 
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));


            // Register JWT Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register Email Service
            services.AddScoped<IEmailService, EmailService>();

            // Register App Options
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.SectionName));


            // Register upload file storage service

            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // Register System Log Service
            services.AddScoped<ISystemLogService, SystemLogService>();

            // Register Audit Context
            services.AddScoped<IAuditContext, AuditContext>();

            // Register Hangfire with SQL Server storage
            services.AddHangfire((serviceProvider, config) =>
            {
                var connectionOptions = serviceProvider.GetRequiredService<IOptions<DatabaseConnectionOptions>>().Value;
                var hangfireOptions = serviceProvider.GetRequiredService<IOptions<HangfireOptions>>().Value;

                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(
                          connectionOptions.DefaultConnection,
                          new SqlServerStorageOptions
                          {
                              CommandBatchMaxTimeout = TimeSpan.FromSeconds(hangfireOptions.CommandBatchMaxTimeoutSeconds),
                              SlidingInvisibilityTimeout = TimeSpan.FromSeconds(hangfireOptions.SlidingInvisibilityTimeoutSeconds),
                              QueuePollInterval = TimeSpan.FromMilliseconds(hangfireOptions.QueuePollIntervalMilliseconds),
                              UseRecommendedIsolationLevel = true,
                              DisableGlobalLocks = hangfireOptions.DisableGlobalLocks
                          });
            });

            services.AddHangfireServer();

            // Register Thumbnail Service
            services.AddScoped<IThumbnailService, ThumbnailService>();

            // Register MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                    cfg.Host(rabbitOptions.Host, "/", h =>
                    {
                        h.Username(rabbitOptions.Username);
                        h.Password(rabbitOptions.Password);
                    });
                });
            });

            services.AddMassTransitHostedService();
            services.AddScoped<IMessageBus, MassTransitMessageBus>();


            return services;
        }
    }
}
