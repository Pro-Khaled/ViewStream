using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
//    public class GetDataDeletionRequestByIdQueryHandler : IRequestHandler<GetDataDeletionRequestByIdQuery, BaseResponse<DataDeletionRequestDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetDataDeletionRequestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<DataDeletionRequestDto>> Handle(GetDataDeletionRequestByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.DataDeletionRequests.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<DataDeletionRequestDto>.Fail("DataDeletionRequest not found");
//                
//                var dto = _mapper.Map<DataDeletionRequestDto>(entity);
//                return BaseResponse<DataDeletionRequestDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<DataDeletionRequestDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
