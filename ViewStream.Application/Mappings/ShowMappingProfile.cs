using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class ShowMappingProfile : MappingProfile
    {
        public ShowMappingProfile()
        {
            // Entity → ShowDto
            CreateMap<Show, ShowDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name)))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
                .ForMember(dest => dest.SeasonCount, opt => opt.MapFrom(src => src.Seasons.Count(s => s.IsDeleted != true)))
                .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.Seasons
                    .Where(s => s.IsDeleted != true)
                    .SelectMany(s => s.Episodes)
                    .Count(e => e.IsDeleted != true)));

            // Entity → ShowListItemDto
            CreateMap<Show, ShowListItemDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name)));

            // CreateShowDto → Show (ignore navigation collections)
            CreateMap<CreateShowDto, Show>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // UpdateShowDto → Show
            CreateMap<UpdateShowDto, Show>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<Show, AdminShowListItemDto>()
            .ForMember(d => d.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name).ToList()))
            .ForMember(d => d.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted ?? false))
            .ForMember(d => d.SeasonCount, opt => opt.Ignore())
            .ForMember(d => d.EpisodeCount, opt => opt.Ignore());
}
    }
}

