using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    using Profile = Domain.Entities.Profile;

    public class ProfileMappingProfile : MappingProfile
    {
        public ProfileMappingProfile()
        {
            CreateMap<Profile, ProfileDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<Profile, ProfileListItemDto>();

            CreateMap<CreateProfileDto, Profile>();
            CreateMap<UpdateProfileDto, Profile>();
        }
    }
}
