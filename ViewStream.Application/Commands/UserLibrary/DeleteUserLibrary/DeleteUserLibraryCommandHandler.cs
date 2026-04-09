using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary
{
//    public class DeleteUserLibraryCommandHandler : IRequestHandler<DeleteUserLibraryCommand, BaseResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public DeleteUserLibraryCommandHandler(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public async Task<BaseResponse<bool>> Handle(DeleteUserLibraryCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserLibrarys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<bool>.Fail("UserLibrary not found");
//                
//                _unitOfWork.UserLibrarys.Remove(entity);
//                await _unitOfWork.SaveChangesAsync();
//                
//                return BaseResponse<bool>.Ok(true, "UserLibrary deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<bool>.Fail($"Error deleting : {ex.Message}");
//            }
//        }
//    }
}
