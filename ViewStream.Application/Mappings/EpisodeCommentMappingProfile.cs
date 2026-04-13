using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class EpisodeCommentMappingProfile : MappingProfile
    {
        public EpisodeCommentMappingProfile()
        {
            CreateMap<EpisodeComment, EpisodeCommentDto>()
                .ForMember(dest => dest.EpisodeTitle, opt => opt.MapFrom(src => src.Episode.Title))
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ProfileAvatar, opt => opt.MapFrom(src => src.Profile.AvatarIcon))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.CommentLikes.Count))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.InverseParentComment.Count(r => r.IsDeleted != true)))
                .ForMember(dest => dest.Replies, opt => opt.Ignore());

            CreateMap<EpisodeComment, EpisodeCommentListItemDto>()
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.ProfileAvatar, opt => opt.MapFrom(src => src.Profile.AvatarIcon))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.CommentLikes.Count))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.InverseParentComment.Count(r => r.IsDeleted != true)));

            CreateMap<CreateEpisodeCommentDto, EpisodeComment>();
            CreateMap<UpdateEpisodeCommentDto, EpisodeComment>();
        }
    }
}
