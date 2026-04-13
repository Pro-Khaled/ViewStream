using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class CommentLikeMappingProfile : MappingProfile
    {
        public CommentLikeMappingProfile()
        {
            CreateMap<CommentLike, CommentLikeDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name));
        }
    }
}
