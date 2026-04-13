using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class ShowAvailabilityMappingProfile : MappingProfile
    {
        public ShowAvailabilityMappingProfile()
        {
            CreateMap<ShowAvailability, ShowAvailabilityDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryCodeNavigation.Name));

            CreateMap<ShowAvailability, ShowAvailabilityListItemDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryCodeNavigation.Name));

            CreateMap<CreateShowAvailabilityDto, ShowAvailability>();
            CreateMap<UpdateShowAvailabilityDto, ShowAvailability>();
        }
    }
}
