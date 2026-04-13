using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class GenreMappingProfile : MappingProfile
    {
        public GenreMappingProfile()
        {
            CreateMap<Genre, GenreDto>()
                .ForMember(dest => dest.ShowCount, opt => opt.MapFrom(src => src.Shows.Count));

            CreateMap<Genre, GenreListItemDto>()
                .ForMember(dest => dest.ShowCount, opt => opt.MapFrom(src => src.Shows.Count));

            CreateMap<CreateGenreDto, Genre>();
            CreateMap<UpdateGenreDto, Genre>();
        }
    }
}
