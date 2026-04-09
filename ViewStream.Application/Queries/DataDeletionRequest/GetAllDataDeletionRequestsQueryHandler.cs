using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
//    public class GetAllDataDeletionRequestsQueryHandler : IRequestHandler<GetAllDataDeletionRequestsQuery, BaseResponse<PagedResult<DataDeletionRequestDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllDataDeletionRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<DataDeletionRequestDto>>> Handle(GetAllDataDeletionRequestsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.DataDeletionRequests.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<DataDeletionRequestDto>>(entityList);
//                var result = new PagedResult<DataDeletionRequestDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<DataDeletionRequestDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<DataDeletionRequestDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
