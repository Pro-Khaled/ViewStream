using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentReport
{
//    public class GetCommentReportByIdQueryHandler : IRequestHandler<GetCommentReportByIdQuery, BaseResponse<CommentReportDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetCommentReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CommentReportDto>> Handle(GetCommentReportByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.CommentReports.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CommentReportDto>.Fail("CommentReport not found");
//                
//                var dto = _mapper.Map<CommentReportDto>(entity);
//                return BaseResponse<CommentReportDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CommentReportDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
