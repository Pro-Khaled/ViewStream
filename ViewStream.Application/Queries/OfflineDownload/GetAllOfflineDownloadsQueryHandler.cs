using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.OfflineDownload
{
//    public class GetAllOfflineDownloadsQueryHandler : IRequestHandler<GetAllOfflineDownloadsQuery, BaseResponse<PagedResult<OfflineDownloadDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllOfflineDownloadsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<OfflineDownloadDto>>> Handle(GetAllOfflineDownloadsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.OfflineDownloads.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<OfflineDownloadDto>>(entityList);
//                var result = new PagedResult<OfflineDownloadDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<OfflineDownloadDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<OfflineDownloadDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
