using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePaymentMethodCommandHandler> _logger;

        public DeletePaymentMethodCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePaymentMethodCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting payment method Id: {Id} for UserId: {UserId}", request.Id, request.UserId);

            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null || method.UserId != request.UserId)
            {
                _logger.LogWarning("Payment method not found or access denied. Id: {Id}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<PaymentMethodDto>(method);
            _unitOfWork.PaymentMethods.Delete(method);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PaymentMethod, object>(
                tableName: "PaymentMethods",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Payment method deleted. Id: {Id}", request.Id);
            return true;
        }
    }
}
