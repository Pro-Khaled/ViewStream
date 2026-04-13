using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class UserInteractionMappingProfile : MappingProfile
    {
        public UserInteractionMappingProfile()
        {
            CreateMap<UserInteraction, UserInteractionDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title));

            CreateMap<UserInteraction, UserInteractionListItemDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title));
        }
    }
}
