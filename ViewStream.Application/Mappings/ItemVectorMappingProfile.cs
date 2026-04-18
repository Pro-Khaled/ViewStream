using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class ItemVectorMappingProfile : MappingProfile
    {
          public ItemVectorMappingProfile()
        {
            CreateMap<ItemVector, ItemVectorDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title));
        }
    }
}
