using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.DeleteCredit
{
    public class DeleteCreditCommandHandler : IRequestHandler<DeleteCreditCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCreditCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(DeleteCreditCommand request, CancellationToken cancellationToken)
        {
            var credit = await _unitOfWork.Credits.GetByIdAsync<long>(request.Id, cancellationToken);
            if (credit == null) return false;
            _unitOfWork.Credits.Delete(credit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
