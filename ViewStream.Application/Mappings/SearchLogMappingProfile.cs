using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class SearchLogMappingProfile : MappingProfile
    {
          public SearchLogMappingProfile()
        {
            CreateMap<SearchLog, SearchLogDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Name : null))
                .ForMember(dest => dest.ClickedShowTitle, opt => opt.MapFrom(src => src.ClickedShow != null ? src.ClickedShow.Title : null));

            CreateMap<SearchLog, SearchLogListItemDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Name : null))
                .ForMember(dest => dest.ClickedShowTitle, opt => opt.MapFrom(src => src.ClickedShow != null ? src.ClickedShow.Title : null));
        }
    }
}
