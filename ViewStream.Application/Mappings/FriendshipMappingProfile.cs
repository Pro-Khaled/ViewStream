using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;

namespace ViewStream.Application.Mappings
{
    public class FriendshipMappingProfile : MappingProfile
    {
        public FriendshipMappingProfile()
        {
            CreateMap<Friendship, FriendshipDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src =>
                    src.User.Profiles.Where(p => p.IsDeleted != true).Select(p => p.AvatarIcon).FirstOrDefault()))
                .ForMember(dest => dest.FriendName, opt => opt.MapFrom(src => src.Friend.UserName))
                .ForMember(dest => dest.FriendFullName, opt => opt.MapFrom(src => src.Friend.FullName))
                .ForMember(dest => dest.FriendAvatar, opt => opt.MapFrom(src =>
                    src.User.Profiles.Where(p => p.IsDeleted != true).Select(p => p.AvatarIcon).FirstOrDefault()));

            CreateMap<Friendship, FriendshipListItemDto>()
                .ForMember(dest => dest.FriendId, opt => opt.MapFrom(src => src.FriendId))
                .ForMember(dest => dest.FriendName, opt => opt.MapFrom(src => src.Friend.UserName))
                .ForMember(dest => dest.FriendFullName, opt => opt.MapFrom(src => src.Friend.FullName))
                .ForMember(dest => dest.FriendAvatar, opt => opt.MapFrom(src =>
                    src.User.Profiles.Where(p => p.IsDeleted != true).Select(p => p.AvatarIcon).FirstOrDefault()))
                .ForMember(dest => dest.IsIncoming, opt => opt.MapFrom(src => false));
        }
    }
}
