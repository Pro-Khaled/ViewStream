using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAward.RemoveShowAward
{
    using ShowAward = ViewStream.Domain.Entities.ShowAward;
    public class RemoveShowAwardCommandHandler : IRequestHandler<RemoveShowAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RemoveShowAwardCommandHandler> _logger;

        public RemoveShowAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RemoveShowAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveShowAwardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing AwardId: {AwardId} from ShowId: {ShowId}", request.AwardId, request.ShowId);

            var items = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == request.ShowId && sa.AwardId == request.AwardId,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            if (item == null)
            {
                _logger.LogWarning("Award assignment not found. ShowId: {ShowId}, AwardId: {AwardId}", request.ShowId, request.AwardId);
                return false;
            }

            var oldValues = _mapper.Map<ShowAwardDto>(item);
            _unitOfWork.ShowAwards.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ShowAward, object>(
                tableName: "ShowAwards",
                recordId: request.ShowId.GetHashCode() ^ request.AwardId,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Award {AwardId} removed from Show {ShowId}", request.AwardId, request.ShowId);
            return true;
        }
    }
}
