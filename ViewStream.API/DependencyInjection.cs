using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using ViewStream.API.Extensions;
using ViewStream.API.Services.Hubs;
using ViewStream.Application;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Domain;
using ViewStream.Infrastructure;
using ViewStream.Shared;



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

            // SignalR service registration
            services.AddSignalR();

            // Add SignalR Hubs and Clients
            services.AddScoped<IEpisodeHubClient, EpisodeHubClient>();

            services.AddScoped<IShowHubClient, ShowHubClient>();

            // Add Redis caching (including distributed cache and MediatR behavior)
            services.AddRedisCaching(configuration);



            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                {
                    policy.WithOrigins("https://localhost:7259", "http://localhost:5014") // Client URL 
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add Rate Limiting
            services.AddRateLimiting(configuration);

            // Add API Versioning
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            return services;
        }

        public static IApplicationBuilder UseApi(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Use Swagger with UI
            app.UseSwagger();           // This generates the swagger.json
            app.UseSwaggerWithUI(configuration);   // This shows the UI

            // Use JWT Authentication
            app.UseJwtAuthentication();

            app.UseCors("AllowClient");


            app.UseAuthentication();

            app.UseAuthorization();



            // Serve static files if client is in same project
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRateLimiter();

            return app;
        }
    }
}
