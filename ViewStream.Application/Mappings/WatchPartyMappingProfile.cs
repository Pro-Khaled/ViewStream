using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class WatchPartyMappingProfile : MappingProfile
    {
        public WatchPartyMappingProfile()
        {
            CreateMap<WatchParty, WatchPartyDto>()
                .ForMember(dest => dest.HostProfileName, opt => opt.MapFrom(src => src.HostProfile.Name))
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Episode.Season.Show.Title))
                .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.WatchPartyParticipants.Count(p => p.LeftAt == null)));

            CreateMap<WatchParty, WatchPartyListItemDto>()
                .ForMember(dest => dest.HostProfileName, opt => opt.MapFrom(src => src.HostProfile.Name))
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Episode.Season.Show.Title))
                .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.WatchPartyParticipants.Count(p => p.LeftAt == null)));
        }
    }
}
