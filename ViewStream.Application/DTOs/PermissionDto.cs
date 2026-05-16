using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a permission assigned to a user role.
    /// </summary>
    public class PermissionDto
    {
        /// <summary>Gets or sets the unique identifier of the permission.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the permission (e.g. "ManageUsers").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the functional group this permission belongs to.</summary>
        public string? GroupName { get; set; }

        /// <summary>Gets or sets a detailed description of what the permission allows.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the permission was created.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a permission.
    /// </summary>
    public class PermissionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the permission.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the permission.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the functional group this permission belongs to.</summary>
        public string? GroupName { get; set; }
    }

    /// <summary>
    /// Request body for creating a new permission.
    /// </summary>
    public class CreatePermissionDto
    {
        /// <summary>Gets or sets the name of the permission. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group name. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "GroupName cannot exceed 50 characters.")]
        public string? GroupName { get; set; }

        /// <summary>Gets or sets the detailed description. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing permission.
    /// </summary>
    public class UpdatePermissionDto
    {
        /// <summary>Gets or sets the updated name. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated group name. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "GroupName cannot exceed 50 characters.")]
        public string? GroupName { get; set; }

        /// <summary>Gets or sets the updated description. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
