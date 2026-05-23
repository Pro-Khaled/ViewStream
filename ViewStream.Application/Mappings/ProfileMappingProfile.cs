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
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted ?? false))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt));

            CreateMap<Profile, ProfileListItemDto>();

            CreateMap<CreateProfileDto, Profile>();
            CreateMap<UpdateProfileDto, Profile>();
            CreateMap<Profile, AdminProfileListItemDto>();

        }
    }
}

