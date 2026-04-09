using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ItemVector
{
//    public class GetItemVectorByIdQueryHandler : IRequestHandler<GetItemVectorByIdQuery, BaseResponse<ItemVectorDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetItemVectorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ItemVectorDto>> Handle(GetItemVectorByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ItemVectors.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ItemVectorDto>.Fail("ItemVector not found");
//                
//                var dto = _mapper.Map<ItemVectorDto>(entity);
//                return BaseResponse<ItemVectorDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ItemVectorDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
