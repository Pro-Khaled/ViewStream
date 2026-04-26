using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList
{
    using SharedListItem = ViewStream.Domain.Entities.SharedListItem;
    public class RemoveShowFromSharedListCommandHandler : IRequestHandler<RemoveShowFromSharedListCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RemoveShowFromSharedListCommandHandler> _logger;

        public RemoveShowFromSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RemoveShowFromSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveShowFromSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing ShowId: {ShowId} from ListId: {ListId} by ProfileId: {ProfileId}",
                request.ShowId, request.ListId, request.ProfileId);

            var items = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == request.ListId && i.ShowId == request.ShowId,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            if (item == null)
            {
                _logger.LogWarning("Item not found. ListId: {ListId}, ShowId: {ShowId}", request.ListId, request.ShowId);
                return false;
            }

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.ListId, cancellationToken);
            if (list == null || (list.OwnerProfileId != request.ProfileId && item.AddedByProfileId != request.ProfileId))
            {
                _logger.LogWarning("Permission denied for removing item. ListId: {ListId}, ProfileId: {ProfileId}",
                    request.ListId, request.ProfileId);
                return false;
            }

            var oldValues = _mapper.Map<SharedListItemDto>(item);
            _unitOfWork.SharedListItems.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedListItem, object>(
                tableName: "SharedListItems",
                recordId: request.ListId.GetHashCode() ^ request.ShowId,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Show {ShowId} removed from List {ListId}", request.ShowId, request.ListId);
            return true;
        }
    }
}
