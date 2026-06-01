using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedListAdmin
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class DeleteSharedListAdminCommandHandler : IRequestHandler<DeleteSharedListAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteSharedListAdminCommandHandler> _logger;

        public DeleteSharedListAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteSharedListAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSharedListAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin soft-deleting shared list Id: {ListId} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.IsDeleted == true)
            {
                _logger.LogWarning("List not found or already deleted. Id: {ListId}", request.Id);
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
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Shared list soft-deleted by admin. Id: {ListId}", list.Id);
            return true;
        }
    }
}
