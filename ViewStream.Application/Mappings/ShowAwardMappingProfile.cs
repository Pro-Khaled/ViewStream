using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class ShowAwardMappingProfile : MappingProfile
    {
        public ShowAwardMappingProfile()
        {
            CreateMap<ShowAward, ShowAwardDto>()
                .ForMember(dest => dest.ShowTitle, opt => opt.MapFrom(src => src.Show.Title))
                .ForMember(dest => dest.AwardName, opt => opt.MapFrom(src => src.Award.Name))
                .ForMember(dest => dest.AwardCategory, opt => opt.MapFrom(src => src.Award.Category))
                .ForMember(dest => dest.AwardYear, opt => opt.MapFrom(src => src.Award.Year));
        }
    }
}
