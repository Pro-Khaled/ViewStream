using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PersonAwardMappingProfile : MappingProfile
    {
        public PersonAwardMappingProfile()
        {
            CreateMap<PersonAward, PersonAwardDto>()
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Person.Name))
                .ForMember(dest => dest.AwardName, opt => opt.MapFrom(src => src.Award.Name))
                .ForMember(dest => dest.AwardCategory, opt => opt.MapFrom(src => src.Award.Category))
                .ForMember(dest => dest.AwardYear, opt => opt.MapFrom(src => src.Award.Year));
        }
    }
}
