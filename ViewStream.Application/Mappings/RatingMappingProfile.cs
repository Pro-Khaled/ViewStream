using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class RatingMappingProfile : MappingProfile
    {
        public RatingMappingProfile()
        {
            CreateMap<Rating, RatingDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating1));

            CreateMap<Rating, RatingListItemDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating1));
        }
    }
}
