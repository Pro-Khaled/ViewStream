using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod
{
    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeletePaymentMethodCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null || method.UserId != request.UserId) return false;
            _unitOfWork.PaymentMethods.Delete(method);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
