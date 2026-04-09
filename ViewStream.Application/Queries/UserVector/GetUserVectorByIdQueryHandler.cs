using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserVector
{
//    public class GetUserVectorByIdQueryHandler : IRequestHandler<GetUserVectorByIdQuery, BaseResponse<UserVectorDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserVectorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserVectorDto>> Handle(GetUserVectorByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserVectors.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserVectorDto>.Fail("UserVector not found");
//                
//                var dto = _mapper.Map<UserVectorDto>(entity);
//                return BaseResponse<UserVectorDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserVectorDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
