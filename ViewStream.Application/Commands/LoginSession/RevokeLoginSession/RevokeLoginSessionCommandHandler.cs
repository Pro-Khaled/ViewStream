using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSession
{
    using LoginSession = ViewStream.Domain.Entities.LoginSession;
    public class RevokeLoginSessionCommandHandler : IRequestHandler<RevokeLoginSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RevokeLoginSessionCommandHandler> _logger;

        public RevokeLoginSessionCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<RevokeLoginSessionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RevokeLoginSessionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Revoking session Id: {SessionId} for UserId: {UserId}", request.Id, request.UserId);

            var session = await _unitOfWork.LoginSessions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (session == null || session.UserId != request.UserId || session.RevokedAt != null)
            {
                _logger.LogWarning("Session not found, access denied, or already revoked. Id: {SessionId}", request.Id);
                return false;
            }

            session.RevokedAt = DateTime.UtcNow;
            _unitOfWork.LoginSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<LoginSession, object>(
                tableName: "LoginSessions",
                recordId: session.Id,
                action: "REVOKE",
                oldValues: new { session.RevokedAt },
                newValues: new { RevokedAt = session.RevokedAt },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Session revoked. Id: {SessionId}, UserId: {UserId}", request.Id, request.UserId);
            return true;
        }
    }

}
