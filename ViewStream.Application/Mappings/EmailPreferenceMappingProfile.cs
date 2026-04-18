using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class EmailPreferenceMappingProfile : MappingProfile
    {
          public EmailPreferenceMappingProfile()
        {
            CreateMap<EmailPreference, EmailPreferenceDto>();
        }
    }
}
