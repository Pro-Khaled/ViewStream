using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PersonalizedRowMappingProfile : MappingProfile
    {
          public PersonalizedRowMappingProfile()
        {
            CreateMap<PersonalizedRow, PersonalizedRowDto>()
                .ForMember(dest => dest.ShowIds, opt => opt.Ignore()); // handled manually
        }
    }
}
