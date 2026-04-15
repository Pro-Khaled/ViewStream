using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PersonMappingProfile : MappingProfile
    {
        public PersonMappingProfile()
        {
            CreateMap<Person, PersonDto>()
                .ForMember(dest => dest.CreditCount, opt => opt.MapFrom(src => src.Credits.Count))
                .ForMember(dest => dest.AwardCount, opt => opt.MapFrom(src => src.PersonAwards.Count));

            CreateMap<Person, PersonListItemDto>()
                .ForMember(dest => dest.CreditCount, opt => opt.MapFrom(src => src.Credits.Count));

            CreateMap<CreatePersonDto, Person>();
            CreateMap<UpdatePersonDto, Person>();
        }
    }
}
