using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedList
{
//    public class DeleteSharedListCommandHandler : IRequestHandler<DeleteSharedListCommand, BaseResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public DeleteSharedListCommandHandler(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public async Task<BaseResponse<bool>> Handle(DeleteSharedListCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SharedLists.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<bool>.Fail("SharedList not found");
//                
//                _unitOfWork.SharedLists.Remove(entity);
//                await _unitOfWork.SaveChangesAsync();
//                
//                return BaseResponse<bool>.Ok(true, "SharedList deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<bool>.Fail($"Error deleting : {ex.Message}");
//            }
//        }
//    }
}
