using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Genre
{
//    public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, BaseResponse<GenreDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetGenreByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<GenreDto>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Genres.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<GenreDto>.Fail("Genre not found");
//                
//                var dto = _mapper.Map<GenreDto>(entity);
//                return BaseResponse<GenreDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<GenreDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
