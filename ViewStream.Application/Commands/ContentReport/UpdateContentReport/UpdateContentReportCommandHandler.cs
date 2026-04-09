using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.UpdateContentReport
{
//    public class UpdateContentReportCommandHandler : IRequestHandler<UpdateContentReportCommand, BaseResponse<ContentReportDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateContentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ContentReportDto>> Handle(UpdateContentReportCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ContentReports.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ContentReportDto>.Fail("ContentReport not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ContentReports.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ContentReportDto>(entity);
//                // return BaseResponse<ContentReportDto>.Ok(dto, "ContentReport updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ContentReportDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
