using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions
{
    using LoginSession = ViewStream.Domain.Entities.LoginSession;
    public class RevokeAllUserSessionsCommandHandler : IRequestHandler<RevokeAllUserSessionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RevokeAllUserSessionsCommandHandler> _logger;

        public RevokeAllUserSessionsCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<RevokeAllUserSessionsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RevokeAllUserSessionsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Revoking all sessions for UserId: {UserId}", request.UserId);

            var sessions = await _unitOfWork.LoginSessions.FindAsync(
                s => s.UserId == request.UserId && s.RevokedAt == null,
                cancellationToken: cancellationToken);

            var sessionList = sessions.ToList();
            if (!sessionList.Any())
            {
                _logger.LogInformation("No active sessions found for UserId: {UserId}", request.UserId);
                return true;
            }

            foreach (var session in sessionList)
            {
                session.RevokedAt = DateTime.UtcNow;
                _unitOfWork.LoginSessions.Update(session);

                _auditContext.SetAudit<LoginSession, object>(
                    tableName: "LoginSessions",
                    recordId: session.Id,
                    action: "REVOKE",
                    oldValues: new { session.RevokedAt },
                    newValues: new { RevokedAt = session.RevokedAt },
                    changedByUserId: request.ActorUserId
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoked {Count} sessions for UserId: {UserId}", sessionList.Count, request.UserId);
            return true;
        }
    }
}
