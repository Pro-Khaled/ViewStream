using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.OfflineDownload
{
//    public class GetOfflineDownloadByIdQueryHandler : IRequestHandler<GetOfflineDownloadByIdQuery, BaseResponse<OfflineDownloadDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetOfflineDownloadByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<OfflineDownloadDto>> Handle(GetOfflineDownloadByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.OfflineDownloads.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<OfflineDownloadDto>.Fail("OfflineDownload not found");
//                
//                var dto = _mapper.Map<OfflineDownloadDto>(entity);
//                return BaseResponse<OfflineDownloadDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<OfflineDownloadDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
