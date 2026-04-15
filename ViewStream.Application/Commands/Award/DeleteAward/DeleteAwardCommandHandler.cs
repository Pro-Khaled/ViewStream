using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.DeleteAward
{
    public class DeleteAwardCommandHandler : IRequestHandler<DeleteAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAwardCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(DeleteAwardCommand request, CancellationToken cancellationToken)
        {
            var award = await _unitOfWork.Awards.GetByIdAsync<int>(request.Id, cancellationToken);
            if (award == null) return false;
            _unitOfWork.Awards.Delete(award);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
