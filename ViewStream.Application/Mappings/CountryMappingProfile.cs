using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class CountryMappingProfile : MappingProfile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.AvailabilityCount, opt => opt.MapFrom(src => src.ShowAvailabilities.Count));

            CreateMap<Country, CountryListItemDto>();

            CreateMap<CreateCountryDto, Country>();
            CreateMap<UpdateCountryDto, Country>();
        }
    }
}
