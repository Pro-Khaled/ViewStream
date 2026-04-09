using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AuditLog.UpdateAuditLog
{
//    public class UpdateAuditLogCommandHandler : IRequestHandler<UpdateAuditLogCommand, BaseResponse<AuditLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateAuditLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AuditLogDto>> Handle(UpdateAuditLogCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.AuditLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AuditLogDto>.Fail("AuditLog not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.AuditLogs.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<AuditLogDto>(entity);
//                // return BaseResponse<AuditLogDto>.Ok(dto, "AuditLog updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AuditLogDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
