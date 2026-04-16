using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class LoginSessionMappingProfile : MappingProfile
    {
          public LoginSessionMappingProfile()
        {
            CreateMap<LoginSession, LoginSessionDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device != null ? src.Device.DeviceName : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.RevokedAt == null && src.ExpiresAt > DateTime.UtcNow));

            CreateMap<LoginSession, LoginSessionListItemDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device != null ? src.Device.DeviceName : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.RevokedAt == null && src.ExpiresAt > DateTime.UtcNow));
        }
    }
}
