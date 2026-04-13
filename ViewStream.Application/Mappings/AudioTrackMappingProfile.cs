using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class AudioTrackMappingProfile : MappingProfile
    {
        public AudioTrackMappingProfile()
        {
            CreateMap<AudioTrack, AudioTrackDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title));

            CreateMap<AudioTrack, AudioTrackListItemDto>();

            CreateMap<CreateAudioTrackDto, AudioTrack>();
            CreateMap<UpdateAudioTrackDto, AudioTrack>();
        }
    }
}
