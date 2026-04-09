using ViewStream.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace ViewStream.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                // Get SwaggerOptions from configuration
                var swaggerOptions = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>();
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

                if (swaggerOptions == null)
                {
                    swaggerOptions = new SwaggerOptions();
                }

                // Basic API Info
                c.SwaggerDoc(swaggerOptions.Version, new OpenApiInfo
                {
                    Title = swaggerOptions.Title,
                    Version = swaggerOptions.Version,
                    Description = swaggerOptions.Description,
                    Contact = new OpenApiContact
                    {
                        Name = swaggerOptions.ContactName,
                        Email = swaggerOptions.ContactEmail,
                        Url = string.IsNullOrEmpty(swaggerOptions.ContactUrl) ? null : new Uri(swaggerOptions.ContactUrl)
                    },
                    License = new OpenApiLicense
                    {
                        Name = swaggerOptions.LicenseName,
                        Url = string.IsNullOrEmpty(swaggerOptions.LicenseUrl) ? null : new Uri(swaggerOptions.LicenseUrl)
                    },
                    TermsOfService = string.IsNullOrEmpty(swaggerOptions.TermsOfServiceUrl) ? null : new Uri(swaggerOptions.TermsOfServiceUrl)
                });

                // Add JWT Authentication using JwtOptions
                if (swaggerOptions.EnableJwtAuthentication && jwtOptions != null)
                {
                    // Define the security scheme using JwtOptions
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = $"JWT Authorization header using the Bearer scheme.\n\n" +
                                    $"Issuer: {jwtOptions.Issuer}\n" +
                                    $"Audience: {jwtOptions.Audience}\n" +
                                    $"Expiry: {jwtOptions.ExpiryMinutes} minutes\n\n" +
                                    "Enter 'Bearer' followed by your token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
                    });

                    // Add security requirement
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                // Optional: Include XML comments
                if (swaggerOptions.EnableXmlComments)
                {
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerOptions = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>();

            if (swaggerOptions == null)
            {
                swaggerOptions = new SwaggerOptions();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{swaggerOptions.Version}/swagger.json", $"{swaggerOptions.Title} {swaggerOptions.Version}");
                c.RoutePrefix = swaggerOptions.RoutePrefix;
                c.DocumentTitle = swaggerOptions.Title;
                c.DefaultModelsExpandDepth(swaggerOptions.DefaultModelsExpandDepth);

                if (swaggerOptions.EnableDisplayRequestDuration)
                {
                    c.DisplayRequestDuration();
                }

                if (swaggerOptions.EnableTryItOutByDefault)
                {
                    c.EnableTryItOutByDefault();
                }

                if (swaggerOptions.EnableDeepLinking)
                {
                    c.EnableDeepLinking();
                }
            });

            return app;
        }
    }
}
