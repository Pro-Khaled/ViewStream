using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions
{
    public class RevokeAllUserSessionsCommandHandler : IRequestHandler<RevokeAllUserSessionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevokeAllUserSessionsCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(RevokeAllUserSessionsCommand request, CancellationToken cancellationToken)
        {
            var sessions = await _unitOfWork.LoginSessions.FindAsync(
                s => s.UserId == request.UserId && s.RevokedAt == null,
                cancellationToken: cancellationToken);

            foreach (var session in sessions)
            {
                session.RevokedAt = DateTime.UtcNow;
                _unitOfWork.LoginSessions.Update(session);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
