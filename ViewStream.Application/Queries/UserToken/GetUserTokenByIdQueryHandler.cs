using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserToken
{
//    public class GetUserTokenByIdQueryHandler : IRequestHandler<GetUserTokenByIdQuery, BaseResponse<UserTokenDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserTokenByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserTokenDto>> Handle(GetUserTokenByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserTokens.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserTokenDto>.Fail("UserToken not found");
//                
//                var dto = _mapper.Map<UserTokenDto>(entity);
//                return BaseResponse<UserTokenDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserTokenDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
