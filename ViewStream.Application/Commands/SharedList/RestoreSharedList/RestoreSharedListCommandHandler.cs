using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.RestoreSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;

    /// <summary>
    /// Restores a soft-deleted shared list for admin.
    /// </summary>
    public class RestoreSharedListCommandHandler : IRequestHandler<RestoreSharedListCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreSharedListCommandHandler> _logger;

        public RestoreSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        /// <summary>
        /// Restores the shared list by setting <c>IsDeleted=false</c> and <c>DeletedAt=null</c>,
        /// and updating <c>UpdatedAt</c>. Writes an audit record via <see cref="IAuditContext"/>.
        /// </summary>
        /// <param name="request">Restore request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if restored; otherwise false.</returns>
        public async Task<bool> Handle(RestoreSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Restoring shared list Id: {ListId} by admin user: {AdminUserId}",
                request.Id,
                request.RestoredByUserId);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.IsDeleted != true)
            {
                _logger.LogWarning(
                    "Shared list not found or not deleted. Id: {ListId}",
                    request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SharedListDto>(list);

            list.IsDeleted = false;
            list.DeletedAt = null;

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedList, object>(
                tableName: "SharedLists",
                recordId: list.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.RestoredByUserId);

            _logger.LogInformation("Shared list restored. Id: {ListId}", list.Id);
            return true;
        }
    }
}
