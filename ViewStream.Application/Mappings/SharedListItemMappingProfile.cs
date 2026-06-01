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
            CreateMap<SharedListItem, AdminSharedListItemListItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShowId))
                .ForMember(dest => dest.ListName, opt => opt.MapFrom(src => src.List.Name))
                .ForMember(dest => dest.ListTitle, opt => opt.MapFrom(src => src.List.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.ContentTitle, opt => opt.MapFrom(src => src.Show != null ? src.Show.Title : null))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => "Show"))
                .ForMember(dest => dest.AddedByProfileName, opt => opt.MapFrom(src => src.AddedByProfile != null ? src.AddedByProfile.Name : null))
                .ForMember(dest => dest.AddedByEmail, opt => opt.MapFrom(src => (string?)null))
                .ForMember(dest => dest.AddedByProfileId, opt => opt.MapFrom(src => src.AddedByProfile != null ? src.AddedByProfile.Id : 0));


            CreateMap<SharedListItem, SharedListItemListItemDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.ShowPosterUrl, opt => opt.MapFrom(src => src.Show.PosterUrl));
        }
    }
}

