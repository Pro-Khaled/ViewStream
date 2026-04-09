using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.UpdatePlaybackEvent
{
//    public class UpdatePlaybackEventCommandHandler : IRequestHandler<UpdatePlaybackEventCommand, BaseResponse<PlaybackEventDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePlaybackEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PlaybackEventDto>> Handle(UpdatePlaybackEventCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PlaybackEvents.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PlaybackEventDto>.Fail("PlaybackEvent not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.PlaybackEvents.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PlaybackEventDto>(entity);
//                // return BaseResponse<PlaybackEventDto>.Ok(dto, "PlaybackEvent updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PlaybackEventDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
