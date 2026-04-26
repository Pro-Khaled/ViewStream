using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.CreatePromoCode
{
    using PromoCode = ViewStream.Domain.Entities.PromoCode;
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, PromoCodeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreatePromoCodeCommandHandler> _logger;

        public CreatePromoCodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreatePromoCodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PromoCodeDto> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating promo code: {Code}", request.Dto.Code);

            var existing = await _unitOfWork.PromoCodes.FindAsync(
                p => p.Code == request.Dto.Code,
                cancellationToken: cancellationToken);
            if (existing.Any())
                throw new InvalidOperationException("Promo code already exists.");

            var promo = _mapper.Map<PromoCode>(request.Dto);
            promo.UsedCount = 0;

            await _unitOfWork.PromoCodes.AddAsync(promo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PromoCode, object>(
                tableName: "PromoCodes",
                recordId: promo.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Promo code created with Id: {PromoCodeId}", promo.Id);
            return _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
