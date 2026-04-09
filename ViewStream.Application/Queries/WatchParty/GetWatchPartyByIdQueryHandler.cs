using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchParty
{
//    public class GetWatchPartyByIdQueryHandler : IRequestHandler<GetWatchPartyByIdQuery, BaseResponse<WatchPartyDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetWatchPartyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchPartyDto>> Handle(GetWatchPartyByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchPartys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchPartyDto>.Fail("WatchParty not found");
//                
//                var dto = _mapper.Map<WatchPartyDto>(entity);
//                return BaseResponse<WatchPartyDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchPartyDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
