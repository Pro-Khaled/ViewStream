using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class PaymentMethodMappingProfile : MappingProfile
    {
          public PaymentMethodMappingProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodDto>();
            CreateMap<CreatePaymentMethodDto, PaymentMethod>();
            CreateMap<UpdatePaymentMethodDto, PaymentMethod>();
        }
    }
}
