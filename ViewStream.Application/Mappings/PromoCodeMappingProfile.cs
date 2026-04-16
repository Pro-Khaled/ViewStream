using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class PromoCodeMappingProfile : MappingProfile
    {
          public PromoCodeMappingProfile()
        {
            CreateMap<PromoCode, PromoCodeDto>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src =>
                    src.ValidFrom <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                    (!src.ValidUntil.HasValue || src.ValidUntil.Value >= DateOnly.FromDateTime(DateTime.UtcNow)) &&
                    (!src.MaxUses.HasValue || src.UsedCount < src.MaxUses)))
                .ForMember(dest => dest.RemainingUses, opt => opt.MapFrom(src =>
                    src.MaxUses.HasValue ? src.MaxUses.Value - (src.UsedCount ?? 0) : int.MaxValue));

            CreateMap<PromoCode, PromoCodeListItemDto>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src =>
                    src.ValidFrom <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                    (!src.ValidUntil.HasValue || src.ValidUntil.Value >= DateOnly.FromDateTime(DateTime.UtcNow)) &&
                    (!src.MaxUses.HasValue || src.UsedCount < src.MaxUses)));

            CreateMap<CreatePromoCodeDto, PromoCode>();
            CreateMap<UpdatePromoCodeDto, PromoCode>();
        }
    }
}
