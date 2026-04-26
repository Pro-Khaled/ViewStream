using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdatePaymentMethodCommandHandler> _logger;

        public UpdatePaymentMethodCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdatePaymentMethodCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PaymentMethodDto?> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating payment method Id: {Id} for UserId: {UserId}", request.Id, request.UserId);

            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null || method.UserId != request.UserId)
            {
                _logger.LogWarning("Payment method not found or access denied. Id: {Id}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<PaymentMethodDto>(method);
            _mapper.Map(request.Dto, method);
            _unitOfWork.PaymentMethods.Update(method);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var auditNewValues = new { method.ExpiryMonth, method.ExpiryYear, method.IsDefault };
            _auditContext.SetAudit<PaymentMethod, object>(
                tableName: "PaymentMethods",
                recordId: method.Id,
                action: "UPDATE",
                oldValues: new { oldValues.ExpiryMonth, oldValues.ExpiryYear, oldValues.IsDefault },
                newValues: auditNewValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Payment method updated. Id: {Id}", method.Id);
            return _mapper.Map<PaymentMethodDto>(method);
        }
    }
}
