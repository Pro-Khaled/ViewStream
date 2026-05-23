using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class UserPromoUsageMappingProfile : MappingProfile
    {
          public UserPromoUsageMappingProfile()
        {
            CreateMap<UserPromoUsage, UserPromoUsageDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PromoCodeCode, opt => opt.MapFrom(src => src.PromoCode.Code));
            CreateMap<UserPromoUsage, AdminUserPromoUsageListItemDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PromoCodeCode, opt => opt.MapFrom(src => src.PromoCode.Code))
                .ForMember(dest => dest.PromoCode, opt => opt.MapFrom(src => src.PromoCode.Code))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.UsedAt));

        }
    }
}

