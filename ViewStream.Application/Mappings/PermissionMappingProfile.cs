using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PermissionMappingProfile : MappingProfile
    {
          public PermissionMappingProfile()
        {
            CreateMap<Permission, PermissionDto>();
            CreateMap<Permission, PermissionListItemDto>();
            CreateMap<CreatePermissionDto, Permission>();
            CreateMap<UpdatePermissionDto, Permission>();
        }
    }
}
