using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a security role assigned to a specific user.
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>Gets or sets the ID of the user.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the assigned role.</summary>
        public long RoleId { get; set; }

        /// <summary>Gets or sets the internal name of the role.</summary>
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for assigning a new role to a user.
    /// </summary>
    public class AssignRoleToUserDto
    {
        /// <summary>Gets or sets the ID of the role to assign.</summary>
        [Required(ErrorMessage = "RoleId is required.")]
        public long RoleId { get; set; }
    }
}
