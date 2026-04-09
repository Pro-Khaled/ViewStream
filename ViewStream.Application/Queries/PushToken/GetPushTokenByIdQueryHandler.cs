using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PushToken
{
//    public class GetPushTokenByIdQueryHandler : IRequestHandler<GetPushTokenByIdQuery, BaseResponse<PushTokenDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPushTokenByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PushTokenDto>> Handle(GetPushTokenByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PushTokens.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PushTokenDto>.Fail("PushToken not found");
//                
//                var dto = _mapper.Map<PushTokenDto>(entity);
//                return BaseResponse<PushTokenDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PushTokenDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
