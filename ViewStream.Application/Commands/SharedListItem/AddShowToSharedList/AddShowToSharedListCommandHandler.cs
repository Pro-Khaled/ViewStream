using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.AddShowToSharedList
{
    using SharedListItem = ViewStream.Domain.Entities.SharedListItem;
    public class AddShowToSharedListCommandHandler : IRequestHandler<AddShowToSharedListCommand, SharedListItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AddShowToSharedListCommandHandler> _logger;

        public AddShowToSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AddShowToSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<SharedListItemDto> Handle(AddShowToSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding ShowId: {ShowId} to ListId: {ListId} by ProfileId: {ProfileId}",
                request.Dto.ShowId, request.ListId, request.ProfileId);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.ListId, cancellationToken);
            if (list == null || list.IsDeleted == true)
                throw new InvalidOperationException("List not found.");

            if (list.OwnerProfileId != request.ProfileId && list.IsPublic != true)
                throw new UnauthorizedAccessException("You don't have permission to add items to this list.");

            var existing = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == request.ListId && i.ShowId == request.Dto.ShowId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("Show is already in this list.");

            var item = new SharedListItem
            {
                ListId = request.ListId,
                ShowId = request.Dto.ShowId,
                AddedByProfileId = request.ProfileId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.SharedListItems.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedListItem, object>(
                tableName: "SharedListItems",
                recordId: request.ListId.GetHashCode() ^ request.Dto.ShowId.GetHashCode(),
                action: "INSERT",
                oldValues: null,
                newValues: new { request.ListId, request.Dto.ShowId, AddedByProfileId = request.ProfileId },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Show {ShowId} added to List {ListId}", request.Dto.ShowId, request.ListId);

            var result = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == item.ListId && i.ShowId == item.ShowId,
                include: q => q.Include(i => i.List).Include(i => i.Show).Include(i => i.AddedByProfile),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListItemDto>(result.First());
        }
    }
}
