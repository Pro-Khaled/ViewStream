using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace ViewStream.Application
{
    /// <summary>
    /// Dependency Injection configuration for Application layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR (if you're using CQRS)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            // Register your application services here
            // Example: services.AddScoped<IProductService, ProductService>();

            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}
