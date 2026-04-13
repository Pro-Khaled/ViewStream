using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary
{
    public class DeleteUserLibraryCommandHandler : IRequestHandler<DeleteUserLibraryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserLibraryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteUserLibraryCommand request, CancellationToken cancellationToken)
        {
            var library = await _unitOfWork.UserLibraries.GetByIdAsync<long>(request.Id, cancellationToken);
            if (library == null || library.ProfileId != request.ProfileId)
                return false;

            _unitOfWork.UserLibraries.Delete(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
