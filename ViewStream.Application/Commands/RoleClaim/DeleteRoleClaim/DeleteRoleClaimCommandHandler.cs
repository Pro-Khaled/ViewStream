using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.RoleClaim.DeleteRoleClaim
{
//    public class DeleteRoleClaimCommandHandler : IRequestHandler<DeleteRoleClaimCommand, BaseResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public DeleteRoleClaimCommandHandler(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public async Task<BaseResponse<bool>> Handle(DeleteRoleClaimCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.RoleClaims.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<bool>.Fail("RoleClaim not found");
//                
//                _unitOfWork.RoleClaims.Remove(entity);
//                await _unitOfWork.SaveChangesAsync();
//                
//                return BaseResponse<bool>.Ok(true, "RoleClaim deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<bool>.Fail($"Error deleting : {ex.Message}");
//            }
//        }
//    }
}
