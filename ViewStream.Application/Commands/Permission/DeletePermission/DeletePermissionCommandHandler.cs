using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.DeletePermission
{
    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePermissionCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync<int>(request.Id, cancellationToken);
            if (permission == null) return false;

            _unitOfWork.Permissions.Delete(permission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
