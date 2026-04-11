using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.RestoreShow
{
    public class RestoreShowCommandHandler : IRequestHandler<RestoreShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreShowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreShowCommand request, CancellationToken cancellationToken)
        {
            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.Id, cancellationToken);
            if (show == null || show.IsDeleted != true) return false;

            show.IsDeleted = false;
            show.DeletedAt = null;
            show.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
