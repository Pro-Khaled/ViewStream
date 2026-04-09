using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.LoginSession
{
//    public class GetLoginSessionByIdQueryHandler : IRequestHandler<GetLoginSessionByIdQuery, BaseResponse<LoginSessionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetLoginSessionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<LoginSessionDto>> Handle(GetLoginSessionByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.LoginSessions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<LoginSessionDto>.Fail("LoginSession not found");
//                
//                var dto = _mapper.Map<LoginSessionDto>(entity);
//                return BaseResponse<LoginSessionDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<LoginSessionDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
