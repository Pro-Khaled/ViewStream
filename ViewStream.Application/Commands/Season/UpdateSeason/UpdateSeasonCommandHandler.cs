using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.UpdateSeason
{
    using Season = ViewStream.Domain.Entities.Season;
    public class UpdateSeasonCommandHandler : IRequestHandler<UpdateSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateSeasonCommandHandler> _logger;

        public UpdateSeasonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateSeasonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating season Id: {SeasonId}", request.Id);

            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted == true)
            {
                _logger.LogWarning("Season not found or already deleted. Id: {SeasonId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SeasonDto>(season);
            _mapper.Map(request.Dto, season);
            season.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Seasons.Update(season);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Season, object>(
                tableName: "Seasons",
                recordId: season.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Season updated. Id: {SeasonId}", season.Id);
            return true;
        }
    }
}
