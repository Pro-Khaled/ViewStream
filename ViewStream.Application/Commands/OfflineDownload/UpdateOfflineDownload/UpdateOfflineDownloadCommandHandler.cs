using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.UpdateOfflineDownload
{
//    public class UpdateOfflineDownloadCommandHandler : IRequestHandler<UpdateOfflineDownloadCommand, BaseResponse<OfflineDownloadDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateOfflineDownloadCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<OfflineDownloadDto>> Handle(UpdateOfflineDownloadCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.OfflineDownloads.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<OfflineDownloadDto>.Fail("OfflineDownload not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.OfflineDownloads.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<OfflineDownloadDto>(entity);
//                // return BaseResponse<OfflineDownloadDto>.Ok(dto, "OfflineDownload updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<OfflineDownloadDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
