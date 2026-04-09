using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.UpdateWatchPartyParticipant
{
//    public class UpdateWatchPartyParticipantCommandHandler : IRequestHandler<UpdateWatchPartyParticipantCommand, BaseResponse<WatchPartyParticipantDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateWatchPartyParticipantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchPartyParticipantDto>> Handle(UpdateWatchPartyParticipantCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchPartyParticipants.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchPartyParticipantDto>.Fail("WatchPartyParticipant not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.WatchPartyParticipants.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<WatchPartyParticipantDto>(entity);
//                // return BaseResponse<WatchPartyParticipantDto>.Ok(dto, "WatchPartyParticipant updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchPartyParticipantDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
