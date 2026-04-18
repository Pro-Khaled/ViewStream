using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class UserVectorMappingProfile : MappingProfile
    {
          public UserVectorMappingProfile()
        {
            CreateMap<UserVector, UserVectorDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name));
        }
    }
}
