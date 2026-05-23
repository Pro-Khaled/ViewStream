using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a security role used for Role-Based Access Control (RBAC).
    /// </summary>
    public class RoleDto
    {
        /// <summary>Gets or sets the unique identifier of the role.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the internal name of the role (e.g. "Admin", "Moderator").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a detailed description of the role's purpose.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a protected system role that cannot be deleted.</summary>
        public bool IsSystem { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the role was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the role was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the list of permission IDs granted to this role.</summary>
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Slim list-item DTO for a security role.
    /// </summary>
    public class RoleListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the role.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the internal name of the role.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the role.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a protected system role.</summary>
        public bool IsSystem { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the role was created.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating a new security role.
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>Gets or sets the name of the role. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the role. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the list of permission IDs to grant to the new role.</summary>
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Request body for updating an existing security role.
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>Gets or sets the updated description. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the updated list of permission IDs granted to the role.</summary>
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Admin list-item DTO for roles shown in the admin dashboard.
    /// </summary>
    public class AdminRoleListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the role.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the internal name of the role.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the role.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a protected system role.</summary>
        public bool IsSystem { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the role was created.</summary>
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PermissionCount { get; set; }
    }
}

