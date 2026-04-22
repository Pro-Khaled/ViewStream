using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class ErrorLogMappingProfile : MappingProfile
    {
          public ErrorLogMappingProfile()
        {
            CreateMap<ErrorLog, ErrorLogDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null));

            CreateMap<ErrorLog, ErrorLogListItemDto>();
            CreateMap<CreateErrorLogDto, ErrorLog>();
        }
    }
}
