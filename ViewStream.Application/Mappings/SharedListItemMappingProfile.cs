using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class SharedListItemMappingProfile : MappingProfile
    {
        public SharedListItemMappingProfile()
        {
            CreateMap<SharedListItem, SharedListItemDto>()
                .ForMember(dest => dest.ListName, opt => opt.MapFrom(src => src.List.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Show.PosterUrl))
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => src.Show.ReleaseYear))
                .ForMember(dest => dest.AddedByProfileName, opt => opt.MapFrom(src => src.AddedByProfile != null ? src.AddedByProfile.Name : null));

            CreateMap<SharedListItem, SharedListItemListItemDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Show.PosterUrl));
        }
    }
}
