using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class SubtitleMappingProfile : MappingProfile
    {
        public SubtitleMappingProfile()
        {
            CreateMap<Subtitle, SubtitleDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title));

            CreateMap<Subtitle, SubtitleListItemDto>();

            CreateMap<CreateSubtitleDto, Subtitle>();
            CreateMap<UpdateSubtitleDto, Subtitle>();
        
            CreateMap<Subtitle, AdminSubtitleListItemDto>()
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted ?? false))
                .ForMember(d => d.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title));
        }
    }
}

