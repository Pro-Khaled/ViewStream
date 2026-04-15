using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class CreditMappingProfile : MappingProfile
    {
        public CreditMappingProfile()
        {
            CreateMap<Credit, CreditDto>()
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Person.Name))
                .ForMember(dest => dest.PersonPhotoUrl, opt => opt.MapFrom(src => src.Person.PhotoUrl))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show != null ? src.Show.Title : null))
                .ForMember(dest => dest.SeasonTitle, opt => opt.MapFrom(src => src.Season != null ? src.Season.Title : null))
                .ForMember(dest => dest.SeasonNumber, opt => opt.MapFrom(src => src.Season != null ? (short?)src.Season.SeasonNumber : null))
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode != null ? src.Episode.Title : null))
                .ForMember(dest => dest.EpisodeNumber, opt => opt.MapFrom(src => src.Episode != null ? (short?)src.Episode.EpisodeNumber : null))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src =>
                    src.ShowId != null ? "Show" : src.SeasonId != null ? "Season" : "Episode"))
                .ForMember(dest => dest.TargetTitle, opt => opt.MapFrom(src =>
                    src.Show != null ? src.Show.Title :
                    src.Season != null ? src.Season.Title :
                    src.Episode != null ? src.Episode.Title : ""));

            CreateMap<Credit, CreditListItemDto>()
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Person.Name))
                .ForMember(dest => dest.PersonPhotoUrl, opt => opt.MapFrom(src => src.Person.PhotoUrl))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src =>
                    src.ShowId != null ? "Show" : src.SeasonId != null ? "Season" : "Episode"))
                .ForMember(dest => dest.TargetTitle, opt => opt.MapFrom(src =>
                    src.Show != null ? src.Show.Title :
                    src.Season != null ? $"{src.Season.Show.Title} - S{src.Season.SeasonNumber}" :
                    src.Episode != null ? $"{src.Episode.Season.Show.Title} - S{src.Episode.Season.SeasonNumber} E{src.Episode.EpisodeNumber}" : ""));

            CreateMap<CreateCreditDto, Credit>();
            CreateMap<UpdateCreditDto, Credit>();
        }
    }
}
