using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SharedListItem
{
//    public class GetSharedListItemByIdQueryHandler : IRequestHandler<GetSharedListItemByIdQuery, BaseResponse<SharedListItemDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSharedListItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SharedListItemDto>> Handle(GetSharedListItemByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SharedListItems.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SharedListItemDto>.Fail("SharedListItem not found");
//                
//                var dto = _mapper.Map<SharedListItemDto>(entity);
//                return BaseResponse<SharedListItemDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SharedListItemDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
