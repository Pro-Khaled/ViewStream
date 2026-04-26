using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PushTokenMappingProfile : MappingProfile
    {
          public PushTokenMappingProfile()
          {
                CreateMap<PushToken, PushTokenDto>();
          }
    }
}
