using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ViewStream.Infrastructure;
using ViewStream.Application;
using ViewStream.Domain;
using ViewStream.Shared;
using ViewStream.API.Extensions;



namespace ViewStream.API
{
    /// <summary>
    /// Dependency Injection configuration for API layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services,IConfiguration configuration)
        {

            // Add Shared layer (registers all options including JwtOptions & SwaggerOptions)
            services.AddShared(configuration);

            // Add other layers
            services.AddApplication();
            services.AddDomain();
            services.AddInfrastructure(configuration);

            // Add API specific services
            services.AddControllers();
            services.AddHttpContextAccessor();

            // Add JWT Authentication (uses JwtOptions)
            services.AddJwtAuthentication(configuration);

            // Add Swagger (uses SwaggerOptions & JwtOptions)
            services.AddSwagger(configuration);

            return services;
        }

        public static IApplicationBuilder UseApi(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Use Swagger with UI
            app.UseSwagger();           // This generates the swagger.json
            app.UseSwaggerWithUI(configuration);   // This shows the UI

            // Use JWT Authentication
            app.UseJwtAuthentication();

            return app;
        }
    }
}
