using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.RestoreSeason
{
    using Season = ViewStream.Domain.Entities.Season;
    public class RestoreSeasonCommandHandler : IRequestHandler<RestoreSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreSeasonCommandHandler> _logger;

        public RestoreSeasonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreSeasonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreSeasonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring season Id: {SeasonId}", request.Id);

            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted != true)
            {
                _logger.LogWarning("Season not found or not deleted. Id: {SeasonId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SeasonDto>(season);
            season.IsDeleted = false;
            season.DeletedAt = null;
            season.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Season, object>(
                tableName: "Seasons",
                recordId: season.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Season restored. Id: {SeasonId}", season.Id);
            return true;
        }
    }
}
