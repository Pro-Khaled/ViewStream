using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedList
{
    public class DeleteSharedListCommandHandler : IRequestHandler<DeleteSharedListCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSharedListCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSharedListCommand request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
                return false;

            list.IsDeleted = true;
            list.DeletedAt = DateTime.UtcNow;

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
