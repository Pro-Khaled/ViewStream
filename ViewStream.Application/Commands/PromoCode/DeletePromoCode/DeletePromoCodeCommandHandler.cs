using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.DeletePromoCode
{
    using PromoCode = ViewStream.Domain.Entities.PromoCode;
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePromoCodeCommandHandler> _logger;

        public DeletePromoCodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePromoCodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting promo code Id: {PromoCodeId}", request.Id);

            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(request.Id, cancellationToken);
            if (promo == null)
            {
                _logger.LogWarning("Promo code not found. Id: {PromoCodeId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<PromoCodeDto>(promo);
            _unitOfWork.PromoCodes.Delete(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PromoCode, object>(
                tableName: "PromoCodes",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Promo code deleted. Id: {PromoCodeId}", request.Id);
            return true;
        }
    }
}
