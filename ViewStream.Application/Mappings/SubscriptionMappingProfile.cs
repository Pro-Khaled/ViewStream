using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class SubscriptionMappingProfile : MappingProfile
    {
          public SubscriptionMappingProfile()
        {
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<CreateSubscriptionDto, Subscription>();
            CreateMap<UpdateSubscriptionDto, Subscription>();
        }
    }
}
