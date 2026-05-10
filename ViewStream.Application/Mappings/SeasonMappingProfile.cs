using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class SeasonMappingProfile : MappingProfile
    {
        public SeasonMappingProfile()
        {
            CreateMap<Season, SeasonDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.Episodes.Count(e => e.IsDeleted != true)));

            CreateMap<Season, SeasonListItemDto>()
                .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.Episodes.Count(e => e.IsDeleted != true)));

            CreateMap<CreateSeasonDto, Season>();
            CreateMap<UpdateSeasonDto, Season>();
        
            CreateMap<Season, AdminSeasonListItemDto>()
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted ?? false))
                .ForMember(d => d.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(d => d.EpisodeCount, opt => opt.Ignore());
        }
    }
}

