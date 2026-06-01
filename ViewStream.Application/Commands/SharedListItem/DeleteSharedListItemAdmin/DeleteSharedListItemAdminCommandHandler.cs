using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.DeleteSharedListItemAdmin
{
    using SharedListItem = ViewStream.Domain.Entities.SharedListItem;
    public class DeleteSharedListItemAdminCommandHandler : IRequestHandler<DeleteSharedListItemAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteSharedListItemAdminCommandHandler> _logger;

        public DeleteSharedListItemAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteSharedListItemAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSharedListItemAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting shared list item ShowId: {ShowId} by AdminUserId: {AdminUserId}",
                request.ShowId, request.AdminUserId);

            var items = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ShowId == request.ShowId,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            if (item == null)
            {
                _logger.LogWarning("Shared list item not found. ShowId: {ShowId}", request.ShowId);
                return false;
            }

            var oldValues = _mapper.Map<SharedListItemDto>(item);
            _unitOfWork.SharedListItems.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedListItem, object>(
                tableName: "SharedListItems",
                recordId: item.ListId.GetHashCode() ^ request.ShowId,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Shared list item hard-deleted by admin. ListId: {ListId}, ShowId: {ShowId}",
                item.ListId, request.ShowId);
            return true;
        }
    }
}
