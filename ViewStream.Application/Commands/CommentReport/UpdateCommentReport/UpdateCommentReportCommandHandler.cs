using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.UpdateCommentReport
{
//    public class UpdateCommentReportCommandHandler : IRequestHandler<UpdateCommentReportCommand, BaseResponse<CommentReportDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateCommentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CommentReportDto>> Handle(UpdateCommentReportCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.CommentReports.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CommentReportDto>.Fail("CommentReport not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.CommentReports.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<CommentReportDto>(entity);
//                // return BaseResponse<CommentReportDto>.Ok(dto, "CommentReport updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CommentReportDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
