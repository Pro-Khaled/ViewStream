using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ViewStream.Domain
{
    /// <summary>
    /// Dependency Injection configuration for Domain layer
    /// </summary>
    public static class AddDomainDI
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            // Domain layer typically has no services to register
            // This layer contains only Entities, Value Objects, Enums, and Interfaces
            
            // Register domain services if any
            // services.AddScoped<IDomainService, DomainService>();
            
            // You can also register FluentValidation validators here
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}
