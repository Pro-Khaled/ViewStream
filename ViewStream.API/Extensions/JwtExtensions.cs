using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using ViewStream.Shared.Options;

namespace ViewStream.API.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Get JWT options from configuration
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            if (jwtOptions == null)
            {
                throw new InvalidOperationException("JWT options are not properly configured");
            }

            if (string.IsNullOrEmpty(jwtOptions.Key) || jwtOptions.Key.Length < 32)
            {
                throw new InvalidOperationException("JWT Key must be at least 32 characters long");
            }

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = jwtOptions.RoleClaimType ?? "role";
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidateLifetime = jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds),
                    NameClaimType = jwtOptions.NameClaimType,
                    RoleClaimType = jwtOptions.RoleClaimType
                };

                // For SignalR or WebSockets
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var oldIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (oldIdentity == null) return Task.CompletedTask;

                        // Create a new identity with the correct role claim type
                        var newIdentity = new ClaimsIdentity(
                            oldIdentity.Claims,
                            oldIdentity.AuthenticationType,
                            jwtOptions.NameClaimType ?? ClaimTypes.Name,
                            ClaimTypes.Role);  // Force role claim type to standard .NET Role

                        // Extract role values from the original "role" claims
                        var roleValues = oldIdentity.FindAll(jwtOptions.RoleClaimType)
                            .Select(c => c.Value)
                            .Distinct()
                            .ToList();

                        // Add standard Role claims for each role
                        foreach (var role in roleValues)
                        {
                            newIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }

                        // Replace the principal
                        context.Principal = new ClaimsPrincipal(newIdentity);
                        context.Success();

                        return Task.CompletedTask;
                    },
                
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Add authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("UserOnly", policy =>
                    policy.RequireRole("User", "Admin"));

                options.AddPolicy("RequireAuthenticated", policy =>
                    policy.RequireAuthenticatedUser());
            });

            return services;
        }

        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
