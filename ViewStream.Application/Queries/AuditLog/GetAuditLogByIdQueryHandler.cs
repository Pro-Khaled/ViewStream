using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AuditLog
{
//    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, BaseResponse<AuditLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAuditLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AuditLogDto>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.AuditLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AuditLogDto>.Fail("AuditLog not found");
//                
//                var dto = _mapper.Map<AuditLogDto>(entity);
//                return BaseResponse<AuditLogDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AuditLogDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
