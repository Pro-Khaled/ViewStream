using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;

namespace ViewStream.Application.Commands.WatchParty.ForceCloseWatchParty
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;

    public class ForceCloseWatchPartyCommandHandler : IRequestHandler<ForceCloseWatchPartyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<ForceCloseWatchPartyCommandHandler> _logger;

        public ForceCloseWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<ForceCloseWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(ForceCloseWatchPartyCommand request, CancellationToken cancellationToken)
        {
            var watchParty = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (watchParty == null)
            {
                throw new NotFoundException("WatchParty", request.Id);
            }

            if (watchParty.IsActive == true)
            {
                // Note: No WatchPartyDto found in research, but if needed we would map it for oldValues.
                // Assuming we just want to audit the action for now.

                watchParty.IsActive = false;
                watchParty.EndedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _auditContext.SetAudit<WatchParty, object>(
                    tableName: "WatchParties",
                    recordId: watchParty.Id,
                    action: "FORCE_CLOSE",
                    oldValues: new { IsActive = true },
                    changedByUserId: request.AdminUserId
                );

                _logger.LogInformation("Watch party {WatchPartyId} force-closed by admin.", watchParty.Id);
            }

            return true;
        }
    }
}

