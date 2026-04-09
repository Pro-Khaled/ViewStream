using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.User
{
//    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, BaseResponse<UserDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Users.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserDto>.Fail("User not found");
//                
//                var dto = _mapper.Map<UserDto>(entity);
//                return BaseResponse<UserDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
