using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.CreateSeason
{
    using Season = Domain.Entities.Season;
    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateSeasonCommandHandler> _logger;

        public CreateSeasonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateSeasonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<long> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating season for ShowId: {ShowId}, SeasonNumber: {SeasonNumber}",
                request.Dto.ShowId, request.Dto.SeasonNumber);

            var season = _mapper.Map<Season>(request.Dto);
            season.CreatedAt = DateTime.UtcNow;
            season.IsDeleted = false;

            await _unitOfWork.Seasons.AddAsync(season, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Season, object>(
                tableName: "Seasons",
                recordId: season.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Season created with Id: {SeasonId}", season.Id);
            return season.Id;
        }
    }
}
