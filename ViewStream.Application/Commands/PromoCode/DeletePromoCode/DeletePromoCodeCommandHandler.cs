using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.DeletePromoCode
{
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoCodeCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(request.Id, cancellationToken);
            if (promo == null) return false;

            _unitOfWork.PromoCodes.Delete(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
