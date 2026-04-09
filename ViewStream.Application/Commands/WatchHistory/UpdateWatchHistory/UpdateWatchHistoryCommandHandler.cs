using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.UpdateWatchHistory
{
//    public class UpdateWatchHistoryCommandHandler : IRequestHandler<UpdateWatchHistoryCommand, BaseResponse<WatchHistoryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateWatchHistoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchHistoryDto>> Handle(UpdateWatchHistoryCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchHistorys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchHistoryDto>.Fail("WatchHistory not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.WatchHistorys.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<WatchHistoryDto>(entity);
//                // return BaseResponse<WatchHistoryDto>.Ok(dto, "WatchHistory updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchHistoryDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
