using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class AuditLogMappingProfile : MappingProfile
    {
          public AuditLogMappingProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(dest => dest.ChangedByUserName, opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.FullName : null));

            CreateMap<AuditLog, AuditLogListItemDto>()
                .ForMember(dest => dest.ChangedByUserName, opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.FullName : null));

            CreateMap<CreateAuditLogDto, AuditLog>();
        }
    }
}
