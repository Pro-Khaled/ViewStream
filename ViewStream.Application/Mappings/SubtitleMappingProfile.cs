using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

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
        }
    }
}
