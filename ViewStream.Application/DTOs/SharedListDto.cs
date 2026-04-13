using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SharedListDto
    {
        public long Id { get; set; }
        public long OwnerProfileId { get; set; }
        public string OwnerProfileName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        public string? ShareCode { get; set; }
        public int ItemCount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class SharedListListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string OwnerProfileName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public bool? IsPublic { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateSharedListDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
    }

    public class UpdateSharedListDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
    }


}
