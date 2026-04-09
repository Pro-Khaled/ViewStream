using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchPartyParticipant
{
//    public class GetAllWatchPartyParticipantsQueryHandler : IRequestHandler<GetAllWatchPartyParticipantsQuery, BaseResponse<PagedResult<WatchPartyParticipantDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllWatchPartyParticipantsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<WatchPartyParticipantDto>>> Handle(GetAllWatchPartyParticipantsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.WatchPartyParticipants.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<WatchPartyParticipantDto>>(entityList);
//                var result = new PagedResult<WatchPartyParticipantDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<WatchPartyParticipantDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<WatchPartyParticipantDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
