using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod
{
    public class SetDefaultPaymentMethodCommandHandler : IRequestHandler<SetDefaultPaymentMethodCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SetDefaultPaymentMethodCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(SetDefaultPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var methods = await _unitOfWork.PaymentMethods.FindAsync(p => p.UserId == request.UserId, cancellationToken: cancellationToken);
            foreach (var pm in methods) pm.IsDefault = false;
            var target = methods.FirstOrDefault(p => p.Id == request.Id);
            if (target == null) return false;
            target.IsDefault = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
