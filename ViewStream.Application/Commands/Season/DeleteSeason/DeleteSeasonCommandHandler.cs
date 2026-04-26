using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.DeleteSeason
{
    using Season = ViewStream.Domain.Entities.Season;
    public class DeleteSeasonCommandHandler : IRequestHandler<DeleteSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteSeasonCommandHandler> _logger;

        public DeleteSeasonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteSeasonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting season Id: {SeasonId}", request.Id);

            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted == true)
            {
                _logger.LogWarning("Season not found or already deleted. Id: {SeasonId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SeasonDto>(season);
            season.IsDeleted = true;
            season.DeletedAt = DateTime.UtcNow;
            season.UpdatedAt = DateTime.UtcNow;

            var episodes = await _unitOfWork.Episodes.FindAsync(e => e.SeasonId == request.Id, cancellationToken: cancellationToken);
            foreach (var episode in episodes)
            {
                episode.IsDeleted = true;
                episode.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Season, object>(
                tableName: "Seasons",
                recordId: season.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Season soft-deleted with Id: {SeasonId}", season.Id);
            return true;
        }
    }
}
