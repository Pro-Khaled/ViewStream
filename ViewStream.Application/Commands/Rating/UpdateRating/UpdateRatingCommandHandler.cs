using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.UpdateRating
{
//    public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, BaseResponse<RatingDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateRatingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<RatingDto>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Ratings.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<RatingDto>.Fail("Rating not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Ratings.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<RatingDto>(entity);
//                // return BaseResponse<RatingDto>.Ok(dto, "Rating updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<RatingDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
