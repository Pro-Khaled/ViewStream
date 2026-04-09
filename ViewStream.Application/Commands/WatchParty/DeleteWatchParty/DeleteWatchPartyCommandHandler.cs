using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchParty
{
//    public class DeleteWatchPartyCommandHandler : IRequestHandler<DeleteWatchPartyCommand, BaseResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public DeleteWatchPartyCommandHandler(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public async Task<BaseResponse<bool>> Handle(DeleteWatchPartyCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchPartys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<bool>.Fail("WatchParty not found");
//                
//                _unitOfWork.WatchPartys.Remove(entity);
//                await _unitOfWork.SaveChangesAsync();
//                
//                return BaseResponse<bool>.Ok(true, "WatchParty deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<bool>.Fail($"Error deleting : {ex.Message}");
//            }
//        }
//    }
}
