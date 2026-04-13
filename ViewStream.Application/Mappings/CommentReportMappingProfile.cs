using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class CommentReportMappingProfile : MappingProfile
    {
        public CommentReportMappingProfile()
        {
            CreateMap<CommentReport, CommentReportDto>()
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.Comment.CommentText))
                .ForMember(dest => dest.CommentAuthorName, opt => opt.MapFrom(src => src.Comment.Profile.Name))
                .ForMember(dest => dest.ReportedByProfileName, opt => opt.MapFrom(src => src.ReportedByProfile.Name))
                .ForMember(dest => dest.ReviewedByUserName, opt => opt.MapFrom(src => src.ReviewedByUser != null ? src.ReviewedByUser.FullName : null));

            CreateMap<CommentReport, CommentReportListItemDto>()
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.Comment.CommentText))
                .ForMember(dest => dest.ReportedByProfileName, opt => opt.MapFrom(src => src.ReportedByProfile.Name));
        }
    }
}
