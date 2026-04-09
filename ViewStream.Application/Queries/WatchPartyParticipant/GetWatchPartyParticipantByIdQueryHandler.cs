using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchPartyParticipant
{
//    public class GetWatchPartyParticipantByIdQueryHandler : IRequestHandler<GetWatchPartyParticipantByIdQuery, BaseResponse<WatchPartyParticipantDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetWatchPartyParticipantByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchPartyParticipantDto>> Handle(GetWatchPartyParticipantByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchPartyParticipants.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchPartyParticipantDto>.Fail("WatchPartyParticipant not found");
//                
//                var dto = _mapper.Map<WatchPartyParticipantDto>(entity);
//                return BaseResponse<WatchPartyParticipantDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchPartyParticipantDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
