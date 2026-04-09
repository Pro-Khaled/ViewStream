using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SearchLog
{
//    public class GetSearchLogByIdQueryHandler : IRequestHandler<GetSearchLogByIdQuery, BaseResponse<SearchLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSearchLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SearchLogDto>> Handle(GetSearchLogByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SearchLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SearchLogDto>.Fail("SearchLog not found");
//                
//                var dto = _mapper.Map<SearchLogDto>(entity);
//                return BaseResponse<SearchLogDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SearchLogDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
