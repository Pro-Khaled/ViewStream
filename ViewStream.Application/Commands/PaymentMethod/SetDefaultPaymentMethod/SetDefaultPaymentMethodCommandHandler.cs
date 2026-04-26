using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class SetDefaultPaymentMethodCommandHandler : IRequestHandler<SetDefaultPaymentMethodCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<SetDefaultPaymentMethodCommandHandler> _logger;

        public SetDefaultPaymentMethodCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<SetDefaultPaymentMethodCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(SetDefaultPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting default payment method Id: {Id} for UserId: {UserId}", request.Id, request.UserId);

            var methods = await _unitOfWork.PaymentMethods.FindAsync(p => p.UserId == request.UserId, cancellationToken: cancellationToken);
            var methodList = methods.ToList();

            foreach (var pm in methodList)
                pm.IsDefault = false;

            var target = methodList.FirstOrDefault(p => p.Id == request.Id);
            if (target == null)
            {
                _logger.LogWarning("Payment method not found. Id: {Id}", request.Id);
                return false;
            }

            target.IsDefault = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PaymentMethod, object>(
                tableName: "PaymentMethods",
                recordId: target.Id,
                action: "UPDATE",
                oldValues: new { IsDefault = false },
                newValues: new { IsDefault = true },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Default payment method set to Id: {Id}", request.Id);
            return true;
        }
    }
}
