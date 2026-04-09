using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Rating
{
//    public class GetRatingByIdQueryHandler : IRequestHandler<GetRatingByIdQuery, BaseResponse<RatingDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetRatingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<RatingDto>> Handle(GetRatingByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Ratings.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<RatingDto>.Fail("Rating not found");
//                
//                var dto = _mapper.Map<RatingDto>(entity);
//                return BaseResponse<RatingDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<RatingDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
