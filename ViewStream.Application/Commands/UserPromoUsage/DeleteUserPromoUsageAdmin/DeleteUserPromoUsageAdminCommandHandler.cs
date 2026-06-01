using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserPromoUsage.DeleteUserPromoUsageAdmin
{
    using UserPromoUsage = ViewStream.Domain.Entities.UserPromoUsage;
    public class DeleteUserPromoUsageAdminCommandHandler : IRequestHandler<DeleteUserPromoUsageAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteUserPromoUsageAdminCommandHandler> _logger;

        public class UserPromoUsageAuditDto
        {
            public long UserId { get; set; }
            public int PromoCodeId { get; set; }
            public DateTime? UsedAt { get; set; }
        }

        public DeleteUserPromoUsageAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteUserPromoUsageAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUserPromoUsageAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting user promo usage for UserId: {UserId}, PromoCodeId: {PromoCodeId} by AdminUserId: {AdminUserId}",
                request.UserId, request.PromoCodeId, request.AdminUserId);

            var usages = await _unitOfWork.UserPromoUsages.FindAsync(
                u => u.UserId == request.UserId && u.PromoCodeId == request.PromoCodeId,
                cancellationToken: cancellationToken);

            var usage = usages.FirstOrDefault();
            if (usage == null)
            {
                _logger.LogWarning("User promo usage not found. UserId: {UserId}, PromoCodeId: {PromoCodeId}",
                    request.UserId, request.PromoCodeId);
                return false;
            }

            var oldValues = new UserPromoUsageAuditDto
            {
                UserId = usage.UserId,
                PromoCodeId = usage.PromoCodeId,
                UsedAt = usage.UsedAt
            };

            _unitOfWork.UserPromoUsages.Delete(usage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserPromoUsage, object>(
                tableName: "UserPromoUsage",
                recordId: 0, // Composite key has no single numeric record ID, so we use 0 or default
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("User promo usage deleted by admin. UserId: {UserId}, PromoCodeId: {PromoCodeId}",
                request.UserId, request.PromoCodeId);
            return true;
        }
    }
}
