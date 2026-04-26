using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class DeleteSharedListCommandHandler : IRequestHandler<DeleteSharedListCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteSharedListCommandHandler> _logger;

        public DeleteSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting shared list Id: {ListId}", request.Id);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
            {
                _logger.LogWarning("List not found or access denied. Id: {ListId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SharedListDto>(list);
            list.IsDeleted = true;
            list.DeletedAt = DateTime.UtcNow;

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedList, object>(
                tableName: "SharedLists",
                recordId: list.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Shared list soft-deleted. Id: {ListId}", list.Id);
            return true;
        }
    }
}
