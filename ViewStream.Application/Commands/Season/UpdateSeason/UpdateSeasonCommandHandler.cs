using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.UpdateSeason
{
//    public class UpdateSeasonCommandHandler : IRequestHandler<UpdateSeasonCommand, BaseResponse<SeasonDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSeasonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SeasonDto>> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Seasons.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SeasonDto>.Fail("Season not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Seasons.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SeasonDto>(entity);
//                // return BaseResponse<SeasonDto>.Ok(dto, "Season updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SeasonDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
