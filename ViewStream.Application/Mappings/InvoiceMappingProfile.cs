using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class InvoiceMappingProfile : MappingProfile
    {
          public InvoiceMappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.MapFrom(src => src.Subscription != null ? src.Subscription.PlanType : null));
            CreateMap<Invoice, InvoiceListItemDto>();
        }
    }
}
