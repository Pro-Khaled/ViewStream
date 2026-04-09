using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SharedList
{
//    public class GetSharedListByIdQueryHandler : IRequestHandler<GetSharedListByIdQuery, BaseResponse<SharedListDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSharedListByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SharedListDto>> Handle(GetSharedListByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SharedLists.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SharedListDto>.Fail("SharedList not found");
//                
//                var dto = _mapper.Map<SharedListDto>(entity);
//                return BaseResponse<SharedListDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SharedListDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
