using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class ContentTagMappingProfile : MappingProfile
    {
        public ContentTagMappingProfile()
        {
            CreateMap<ContentTag, ContentTagDto>()
                .ForMember(dest => dest.ShowCount, opt => opt.MapFrom(src => src.Shows.Count));

            CreateMap<ContentTag, ContentTagListItemDto>()
                .ForMember(dest => dest.ShowCount, opt => opt.MapFrom(src => src.Shows.Count));

            CreateMap<CreateContentTagDto, ContentTag>();
            CreateMap<UpdateContentTagDto, ContentTag>();
        }
    }
}
