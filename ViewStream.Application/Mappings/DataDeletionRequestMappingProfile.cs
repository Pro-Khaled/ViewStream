using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class DataDeletionRequestMappingProfile : MappingProfile
    {
          public DataDeletionRequestMappingProfile()
        {
            CreateMap<DataDeletionRequest, DataDeletionRequestDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<DataDeletionRequest, DataDeletionRequestListItemDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
        }
    }
}
