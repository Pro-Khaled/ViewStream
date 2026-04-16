using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSession
{
    public class RevokeLoginSessionCommandHandler : IRequestHandler<RevokeLoginSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevokeLoginSessionCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(RevokeLoginSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.LoginSessions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (session == null || session.UserId != request.UserId || session.RevokedAt != null) return false;

            session.RevokedAt = DateTime.UtcNow;
            _unitOfWork.LoginSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
