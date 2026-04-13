using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class WatchPartyParticipantMappingProfile : MappingProfile
    {
        public WatchPartyParticipantMappingProfile()
        {
            CreateMap<WatchPartyParticipant, WatchPartyParticipantDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.LeftAt == null));
        }
    }
}
