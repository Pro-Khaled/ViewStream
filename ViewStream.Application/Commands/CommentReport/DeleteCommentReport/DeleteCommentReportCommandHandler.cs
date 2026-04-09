using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.DeleteCommentReport
{
//    public class DeleteCommentReportCommandHandler : IRequestHandler<DeleteCommentReportCommand, BaseResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public DeleteCommentReportCommandHandler(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public async Task<BaseResponse<bool>> Handle(DeleteCommentReportCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.CommentReports.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<bool>.Fail("CommentReport not found");
//                
//                _unitOfWork.CommentReports.Remove(entity);
//                await _unitOfWork.SaveChangesAsync();
//                
//                return BaseResponse<bool>.Ok(true, "CommentReport deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<bool>.Fail($"Error deleting : {ex.Message}");
//            }
//        }
//    }
}
