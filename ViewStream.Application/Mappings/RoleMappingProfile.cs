using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class RoleMappingProfile : MappingProfile
    {
          public RoleMappingProfile()
        {
            CreateMap<Role, RoleDto>().ForMember(d => d.PermissionIds, o => o.MapFrom(s => s.Permissions.Select(p => p.Id)));
            CreateMap<Role, RoleListItemDto>();
        }
    }
}
