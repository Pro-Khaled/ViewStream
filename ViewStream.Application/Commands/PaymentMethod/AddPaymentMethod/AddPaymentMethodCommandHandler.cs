using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, PaymentMethodDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AddPaymentMethodCommandHandler> _logger;

        public AddPaymentMethodCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AddPaymentMethodCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PaymentMethodDto> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding payment method for UserId: {UserId}", request.UserId);

            if (request.Dto.IsDefault == true)
            {
                var existing = await _unitOfWork.PaymentMethods.FindAsync(p => p.UserId == request.UserId, cancellationToken: cancellationToken);
                foreach (var pm in existing) pm.IsDefault = false;
            }

            var method = _mapper.Map<PaymentMethod>(request.Dto);
            method.UserId = request.UserId;
            method.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.PaymentMethods.AddAsync(method, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mask sensitive data before auditing
            var auditNewValues = new { method.Id, method.UserId, method.Provider, method.LastFour, method.CardType, method.ExpiryMonth, method.ExpiryYear, method.IsDefault };
            _auditContext.SetAudit<PaymentMethod, object>(
                tableName: "PaymentMethods",
                recordId: method.Id,
                action: "INSERT",
                oldValues: null,
                newValues: auditNewValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Payment method added with Id: {PaymentMethodId}", method.Id);
            return _mapper.Map<PaymentMethodDto>(method);
        }
    }
}
