using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibraryAdmin
{
    using UserLibrary = ViewStream.Domain.Entities.UserLibrary;
    public class DeleteUserLibraryAdminCommandHandler : IRequestHandler<DeleteUserLibraryAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteUserLibraryAdminCommandHandler> _logger;

        public DeleteUserLibraryAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteUserLibraryAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUserLibraryAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting library item Id: {LibraryId} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var library = await _unitOfWork.UserLibraries.GetByIdAsync<long>(request.Id, cancellationToken);
            if (library == null)
            {
                _logger.LogWarning("Library item not found. Id: {LibraryId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<UserLibraryDto>(library);
            _unitOfWork.UserLibraries.Delete(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserLibrary, object>(
                tableName: "UserLibraries",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Library item deleted by admin. Id: {LibraryId}", request.Id);
            return true;
        }
    }
}
