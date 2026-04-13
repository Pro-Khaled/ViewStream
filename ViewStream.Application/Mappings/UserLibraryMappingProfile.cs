using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class UserLibraryMappingProfile : MappingProfile
    {
        public UserLibraryMappingProfile()
        {
            CreateMap<UserLibrary, UserLibraryDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show != null ? src.Show.Title : null))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Show != null ? src.Show.PosterUrl : null))
                .ForMember(dest => dest.SeasonTitle, opt => opt.MapFrom(src => src.Season.Title))
                .ForMember(dest => dest.SeasonNumber, opt => opt.MapFrom(src => src.Season.SeasonNumber));

            CreateMap<UserLibrary, UserLibraryListItemDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.Show != null ? src.Show.Title :
                    src.Season != null ? $"{src.Season.Show.Title} - S{src.Season.SeasonNumber}" : "Unknown"))
                .ForMember(dest => dest.PosterUrl, opt => opt.MapFrom(src =>
                    src.Show != null ? src.Show.PosterUrl :
                    src.Season != null ? src.Season.Show.PosterUrl : null))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ShowId != null ? "Show" : "Season"));
        }
    }
}
