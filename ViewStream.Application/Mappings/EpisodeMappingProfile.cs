using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class EpisodeMappingProfile : MappingProfile
    {
        public EpisodeMappingProfile()
        {
            CreateMap<Episode, EpisodeDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Season.Show.Title))
                .ForMember(dest => dest.SeasonTitle, opt => opt.MapFrom(src => src.Season.Title))
                .ForMember(dest => dest.SeasonNumber, opt => opt.MapFrom(src => src.Season.SeasonNumber));

            CreateMap<Episode, EpisodeListItemDto>();
            CreateMap<CreateEpisodeDto, Episode>();
            CreateMap<UpdateEpisodeDto, Episode>();
        
        CreateMap<Episode, AdminEpisodeListItemDto>()
            .ForMember(d => d.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted ?? false))
            .ForMember(d => d.ShowTitle, opt => opt.MapFrom(src => src.Season.Show.Title))
            .ForMember(d => d.SeasonNumber, opt => opt.MapFrom(src => src.Season.SeasonNumber));
        }
    }
}

