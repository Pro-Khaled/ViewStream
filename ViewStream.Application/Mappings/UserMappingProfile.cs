using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class UserMappingProfile : MappingProfile
    {
          public UserMappingProfile()
          {

        
        CreateMap<User, AdminUserListItemDto>()
            .ForMember(d => d.ProfileCount, opt => opt.Ignore())
            .ForMember(d => d.Roles, opt => opt.Ignore());
        }
    }
}

