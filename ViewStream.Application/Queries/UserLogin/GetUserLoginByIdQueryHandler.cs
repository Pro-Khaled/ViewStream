using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserLogin
{
//    public class GetUserLoginByIdQueryHandler : IRequestHandler<GetUserLoginByIdQuery, BaseResponse<UserLoginDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserLoginByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserLoginDto>> Handle(GetUserLoginByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserLogins.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserLoginDto>.Fail("UserLogin not found");
//                
//                var dto = _mapper.Map<UserLoginDto>(entity);
//                return BaseResponse<UserLoginDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserLoginDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
