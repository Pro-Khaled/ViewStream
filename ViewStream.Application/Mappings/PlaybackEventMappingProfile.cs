using AutoMapper;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class PlaybackEventMappingProfile : MappingProfile
    {
          public PlaybackEventMappingProfile()
          {
//            // Entity → DTO
//            CreateMap<PlaybackEvent, PlaybackEventDto>()
//                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
//                // Add custom mappings for related entities or computed properties here
//                ;
//            
//            // Create DTO → Entity (for Create/Update commands)
//            CreateMap<CreatePlaybackEventDto, PlaybackEvent>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
//                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
//            
//            CreateMap<UpdatePlaybackEventDto, PlaybackEvent>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
//                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
