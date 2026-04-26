using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.DeleteShow
{
    using Show = Domain.Entities.Show;
    public class DeleteShowCommandHandler : IRequestHandler<DeleteShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteShowCommandHandler> _logger;

        public DeleteShowCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteShowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteShowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Soft‑deleting show with Id: {ShowId}", request.Id);

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.Id, cancellationToken);
            if (show == null || show.IsDeleted == true)
            {
                _logger.LogWarning("Show not found or already deleted. Id: {ShowId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ShowDto>(show);
            show.IsDeleted = true;
            show.DeletedAt = DateTime.UtcNow;
            show.UpdatedAt = DateTime.UtcNow;

            // Cascade soft delete to seasons and episodes
            var seasons = await _unitOfWork.Seasons.FindAsync(s => s.ShowId == request.Id, cancellationToken: cancellationToken);
            foreach (var season in seasons)
            {
                season.IsDeleted = true;
                season.DeletedAt = DateTime.UtcNow;
                var episodes = await _unitOfWork.Episodes.FindAsync(e => e.SeasonId == season.Id, cancellationToken: cancellationToken);
                foreach (var episode in episodes)
                {
                    episode.IsDeleted = true;
                    episode.DeletedAt = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Show, object>(
                tableName: "Shows",
                recordId: show.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.DeletedByUserId
            );

            _logger.LogInformation("Show soft‑deleted. Id: {ShowId}", show.Id);
            return true;
        }
    }
}
