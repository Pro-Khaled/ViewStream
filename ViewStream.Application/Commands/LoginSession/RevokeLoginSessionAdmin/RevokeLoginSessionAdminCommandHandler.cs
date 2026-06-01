using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSessionAdmin
{
    using LoginSession = ViewStream.Domain.Entities.LoginSession;
    public class RevokeLoginSessionAdminCommandHandler : IRequestHandler<RevokeLoginSessionAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RevokeLoginSessionAdminCommandHandler> _logger;

        public RevokeLoginSessionAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<RevokeLoginSessionAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RevokeLoginSessionAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin revoking session Id: {SessionId} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var session = await _unitOfWork.LoginSessions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (session == null || session.RevokedAt != null)
            {
                _logger.LogWarning("Session not found or already revoked. Id: {SessionId}", request.Id);
                return false;
            }

            session.RevokedAt = DateTime.UtcNow;
            _unitOfWork.LoginSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<LoginSession, object>(
                tableName: "LoginSessions",
                recordId: session.Id,
                action: "REVOKE_BY_ADMIN",
                oldValues: new { session.RevokedAt },
                newValues: new { RevokedAt = session.RevokedAt },
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Session revoked by admin. Id: {SessionId}", request.Id);
            return true;
        }
    }
}
