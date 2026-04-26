using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.UpdatePromoCode
{
    using PromoCode = ViewStream.Domain.Entities.PromoCode;
    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, PromoCodeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdatePromoCodeCommandHandler> _logger;

        public UpdatePromoCodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdatePromoCodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PromoCodeDto?> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating promo code Id: {PromoCodeId}", request.Id);

            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(request.Id, cancellationToken);
            if (promo == null)
            {
                _logger.LogWarning("Promo code not found. Id: {PromoCodeId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<PromoCodeDto>(promo);
            _mapper.Map(request.Dto, promo);
            _unitOfWork.PromoCodes.Update(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PromoCode, object>(
                tableName: "PromoCodes",
                recordId: promo.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Promo code updated. Id: {PromoCodeId}", promo.Id);
            return _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
