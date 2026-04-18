using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PermissionListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GroupName { get; set; }
    }

    public class CreatePermissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdatePermissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public string? Description { get; set; }
    }
    
}
