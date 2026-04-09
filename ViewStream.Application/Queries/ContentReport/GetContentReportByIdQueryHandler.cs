using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentReport
{
//    public class GetContentReportByIdQueryHandler : IRequestHandler<GetContentReportByIdQuery, BaseResponse<ContentReportDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetContentReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ContentReportDto>> Handle(GetContentReportByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ContentReports.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ContentReportDto>.Fail("ContentReport not found");
//                
//                var dto = _mapper.Map<ContentReportDto>(entity);
//                return BaseResponse<ContentReportDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ContentReportDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
