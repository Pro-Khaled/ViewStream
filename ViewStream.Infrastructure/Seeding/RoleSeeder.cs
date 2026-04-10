using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Entities;

namespace ViewStream.Infrastructure.Seeding
{

    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<RoleManager<Role>>>();

            string[] defaultRoles = { "User", "ContentManager", "Moderator", "SupportAgent", "SuperAdmin" };

            foreach (var roleName in defaultRoles)
            {
                // Use FindByNameAsync instead of RoleExistsAsync for better case-insensitivity
                var existingRole = await roleManager.FindByNameAsync(roleName);
                if (existingRole != null)
                {
                    logger.LogDebug("Role '{RoleName}' already exists. Skipping.", roleName);
                    continue;
                }

                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant(), // Important for Identity lookups
                    Description = GetRoleDescription(roleName),
                    IsSystem = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Use try-catch to handle rare race conditions where another instance creates the role between check and insert
                try
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                    else
                        logger.LogError("Failed to create role '{RoleName}': {Errors}", roleName, string.Join(", ", result.Errors));
                }
                catch (Exception ex) when (ex.InnerException?.Message.Contains("UNIQUE KEY") == true ||
                                            ex.InnerException?.Message.Contains("duplicate key") == true)
                {
                    logger.LogWarning("Role '{RoleName}' was created by another instance. Continuing.", roleName);
                }
            }
        }

        private static string GetRoleDescription(string roleName) => roleName switch
        {
            "User" => "Standard user with basic streaming access",
            "ContentManager" => "Can manage shows, seasons, and episodes",
            "Moderator" => "Can moderate comments and reports",
            "SupportAgent" => "Can view user data and assist with issues",
            "SuperAdmin" => "Full system access",
            _ => string.Empty
        };
    }
}
