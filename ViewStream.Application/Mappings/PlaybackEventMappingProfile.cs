using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PlaybackEventMappingProfile : MappingProfile
    {
          public PlaybackEventMappingProfile()
        {
            CreateMap<PlaybackEvent, PlaybackEventDto>();
            CreateMap<CreatePlaybackEventDto, PlaybackEvent>();
        }
    }
}
