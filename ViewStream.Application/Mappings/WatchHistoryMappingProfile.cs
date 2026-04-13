using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class WatchHistoryMappingProfile : MappingProfile
    {
        public WatchHistoryMappingProfile()
        {
            CreateMap<WatchHistory, WatchHistoryDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.SeasonId, opt => opt.MapFrom(src => src.Episode.SeasonId))
                .ForMember(dest => dest.SeasonNumber, opt => opt.MapFrom(src => src.Episode.Season.SeasonNumber))
                .ForMember(dest => dest.ShowId, opt => opt.MapFrom(src => src.Episode.Season.ShowId))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Episode.Season.Show.Title))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Episode.Season.Show.PosterUrl))
                .ForMember(dest => dest.TotalSeconds, opt => opt.MapFrom(src => src.Episode.RuntimeSeconds));

            CreateMap<WatchHistory, WatchHistoryListItemDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Episode.Season.Show.Title))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Episode.Season.Show.PosterUrl))
                .ForMember(dest => dest.TotalSeconds, opt => opt.MapFrom(src => src.Episode.RuntimeSeconds));
        }
    }
}
