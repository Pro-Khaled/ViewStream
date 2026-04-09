using ViewStream.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace ViewStream.Shared
{
    /// <summary>
    /// Shared Dependency Injection - Cross-cutting concerns
    /// Can be used by any layer (Domain, Application, Infrastructure, API)
    /// </summary>
    public static class ShardDI
    {
        public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
        {
            // ============================================
            // OPTIONS PATTERN - Configuration binding
            // ============================================
            services.Configure<DatabaseConnectionOptions>(
                configuration.GetSection(DatabaseConnectionOptions.SectionName));

            // Register database behavior options
            services.Configure<DatabaseOptions>(
                configuration.GetSection(DatabaseOptions.SectionName));


            return services;
        }
    }
}
