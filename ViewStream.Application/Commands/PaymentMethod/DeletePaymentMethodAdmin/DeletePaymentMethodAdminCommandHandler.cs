using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethodAdmin
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class DeletePaymentMethodAdminCommandHandler : IRequestHandler<DeletePaymentMethodAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePaymentMethodAdminCommandHandler> _logger;

        public DeletePaymentMethodAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePaymentMethodAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePaymentMethodAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting payment method Id: {Id} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null)
            {
                _logger.LogWarning("Payment method not found. Id: {Id}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<PaymentMethodDto>(method);
            _unitOfWork.PaymentMethods.Delete(method);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PaymentMethod, object>(
                tableName: "PaymentMethods",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Payment method deleted by admin. Id: {Id}", request.Id);
            return true;
        }
    }
}
