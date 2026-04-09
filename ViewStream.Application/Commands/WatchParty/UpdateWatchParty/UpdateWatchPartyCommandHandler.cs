using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.UpdateWatchParty
{
//    public class UpdateWatchPartyCommandHandler : IRequestHandler<UpdateWatchPartyCommand, BaseResponse<WatchPartyDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateWatchPartyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchPartyDto>> Handle(UpdateWatchPartyCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchPartys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchPartyDto>.Fail("WatchParty not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.WatchPartys.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<WatchPartyDto>(entity);
//                // return BaseResponse<WatchPartyDto>.Ok(dto, "WatchParty updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchPartyDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
