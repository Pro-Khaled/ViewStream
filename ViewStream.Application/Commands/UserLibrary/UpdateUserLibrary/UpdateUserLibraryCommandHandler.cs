using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary
{
    using UserLibrary = ViewStream.Domain.Entities.UserLibrary;
    public class UpdateUserLibraryCommandHandler : IRequestHandler<UpdateUserLibraryCommand, UserLibraryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateUserLibraryCommandHandler> _logger;

        public UpdateUserLibraryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateUserLibraryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<UserLibraryDto?> Handle(UpdateUserLibraryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating library item Id: {LibraryId}", request.Id);

            var library = await _unitOfWork.UserLibraries.GetByIdAsync<long>(request.Id, cancellationToken);
            if (library == null || library.ProfileId != request.ProfileId)
            {
                _logger.LogWarning("Library item not found or access denied. Id: {LibraryId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<UserLibraryDto>(library);

            var dto = request.Dto;
            if (dto.Status != null) library.Status = dto.Status;
            if (dto.EpisodesWatched.HasValue) library.EpisodesWatched = dto.EpisodesWatched;
            if (dto.UserScore.HasValue) library.UserScore = dto.UserScore;
            if (dto.StartedAt.HasValue) library.StartedAt = dto.StartedAt;
            if (dto.CompletedAt.HasValue) library.CompletedAt = dto.CompletedAt;
            library.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserLibraries.Update(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserLibrary, object>(
                tableName: "UserLibraries",
                recordId: library.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Library item updated. Id: {LibraryId}", library.Id);

            var result = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.Id == library.Id,
                include: q => q.Include(ul => ul.Profile)
                               .Include(ul => ul.Show)
                               .Include(ul => ul.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserLibraryDto>(result.First());
        }
    }
}
