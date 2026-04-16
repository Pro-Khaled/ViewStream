using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class OfflineDownloadMappingProfile : MappingProfile
    {
          public OfflineDownloadMappingProfile()
        {
            CreateMap<OfflineDownload, OfflineDownloadDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.DeviceName));

            CreateMap<OfflineDownload, OfflineDownloadListItemDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title));
        }
    }
}
