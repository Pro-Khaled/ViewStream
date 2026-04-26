using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary
{
    using UserLibrary = ViewStream.Domain.Entities.UserLibrary;
    public class DeleteUserLibraryCommandHandler : IRequestHandler<DeleteUserLibraryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteUserLibraryCommandHandler> _logger;

        public DeleteUserLibraryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteUserLibraryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUserLibraryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting library item Id: {LibraryId}", request.Id);

            var library = await _unitOfWork.UserLibraries.GetByIdAsync<long>(request.Id, cancellationToken);
            if (library == null || library.ProfileId != request.ProfileId)
            {
                _logger.LogWarning("Library item not found or access denied. Id: {LibraryId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<UserLibraryDto>(library);
            _unitOfWork.UserLibraries.Delete(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserLibrary, object>(
                tableName: "UserLibraries",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Library item deleted. Id: {LibraryId}", request.Id);
            return true;
        }
    }

}
