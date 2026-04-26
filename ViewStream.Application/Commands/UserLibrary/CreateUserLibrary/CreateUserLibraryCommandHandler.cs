using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.CreateUserLibrary
{
    using UserLibrary = ViewStream.Domain.Entities.UserLibrary;
    public class CreateUserLibraryCommandHandler : IRequestHandler<CreateUserLibraryCommand, UserLibraryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateUserLibraryCommandHandler> _logger;

        public CreateUserLibraryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateUserLibraryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<UserLibraryDto> Handle(CreateUserLibraryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!dto.ShowId.HasValue && !dto.SeasonId.HasValue)
                throw new ArgumentException("Either ShowId or SeasonId must be provided.");

            _logger.LogInformation("Adding to library: ProfileId={ProfileId}, ShowId={ShowId}, SeasonId={SeasonId}, Status={Status}",
                request.ProfileId, dto.ShowId, dto.SeasonId, dto.Status);

            // Check for existing entry
            var existing = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.ProfileId == request.ProfileId && ul.ShowId == dto.ShowId && ul.SeasonId == dto.SeasonId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("This item is already in your library.");

            var library = new UserLibrary
            {
                ProfileId = request.ProfileId,
                ShowId = dto.ShowId,
                SeasonId = dto.SeasonId,
                Status = dto.Status,
                EpisodesWatched = dto.EpisodesWatched,
                UserScore = dto.UserScore,
                StartedAt = dto.StartedAt,
                CompletedAt = dto.CompletedAt,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserLibraries.AddAsync(library, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserLibrary, object>(
                tableName: "UserLibraries",
                recordId: library.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { library.ProfileId, library.ShowId, library.SeasonId, library.Status, library.UserScore },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Library entry created with Id: {LibraryId}", library.Id);

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
