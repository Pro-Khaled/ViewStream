using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class ContentReportMappingProfile : MappingProfile
    {
        public ContentReportMappingProfile()
        {
            CreateMap<ContentReport, ContentReportDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show != null ? src.Show.Title : null))
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode != null ? src.Episode.Title : null));

            CreateMap<ContentReport, ContentReportListItemDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src => src.ShowId != null ? "Show" : "Episode"))
                .ForMember(dest => dest.TargetTitle, opt => opt.MapFrom(src =>
                    src.ShowId != null ? src.Show.Title :
                    src.Episode != null ? src.Episode.Title : "Unknown"));
        }
    }
}
