using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class RoleListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class UpdateRoleDto
    {
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

}
