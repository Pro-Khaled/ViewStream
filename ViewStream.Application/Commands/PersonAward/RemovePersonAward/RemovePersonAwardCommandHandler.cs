using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.RemovePersonAward
{
    using PersonAward = ViewStream.Domain.Entities.PersonAward;
    public class RemovePersonAwardCommandHandler : IRequestHandler<RemovePersonAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RemovePersonAwardCommandHandler> _logger;

        public RemovePersonAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RemovePersonAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RemovePersonAwardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing AwardId: {AwardId} from PersonId: {PersonId}", request.AwardId, request.PersonId);

            var items = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == request.PersonId && pa.AwardId == request.AwardId,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            if (item == null)
            {
                _logger.LogWarning("Award assignment not found. PersonId: {PersonId}, AwardId: {AwardId}", request.PersonId, request.AwardId);
                return false;
            }

            var oldValues = _mapper.Map<PersonAwardDto>(item);
            _unitOfWork.PersonAwards.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonAward, object>(
                tableName: "PersonAwards",
                recordId: request.PersonId.GetHashCode() ^ request.AwardId,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Award {AwardId} removed from Person {PersonId}", request.AwardId, request.PersonId);
            return true;
        }
    }
}
