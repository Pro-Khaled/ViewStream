using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class AwardMappingProfile : MappingProfile
    {
        public AwardMappingProfile()
        {
            CreateMap<Award, AwardDto>()
                .ForMember(dest => dest.PersonAwardCount, opt => opt.MapFrom(src => src.PersonAwards.Count))
                .ForMember(dest => dest.ShowAwardCount, opt => opt.MapFrom(src => src.ShowAwards.Count));
            CreateMap<Award, AwardListItemDto>();
            CreateMap<CreateAwardDto, Award>();
            CreateMap<UpdateAwardDto, Award>();
        }
    }
}
