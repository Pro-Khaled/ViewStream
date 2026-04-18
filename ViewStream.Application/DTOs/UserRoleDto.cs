using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserRoleDto
    {
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class AssignRoleToUserDto
    {
        public long RoleId { get; set; }
    }

}
