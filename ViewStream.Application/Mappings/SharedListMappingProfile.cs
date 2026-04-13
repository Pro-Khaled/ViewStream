using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class SharedListMappingProfile : MappingProfile
    {
        public SharedListMappingProfile()
        {
            CreateMap<SharedList, SharedListDto>()
                .ForMember(dest => dest.OwnerProfileName, opt => opt.MapFrom(src => src.OwnerProfile.Name))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.SharedListItems.Count));

            CreateMap<SharedList, SharedListListItemDto>()
                .ForMember(dest => dest.OwnerProfileName, opt => opt.MapFrom(src => src.OwnerProfile.Name))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.SharedListItems.Count));
        }
    }
}
