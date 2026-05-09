using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.RegenerateRecommendations
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;

    public class RegenerateRecommendationsCommandHandler
        : IRequestHandler<RegenerateRecommendationsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RegenerateRecommendationsCommandHandler> _logger;

        public RegenerateRecommendationsCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<RegenerateRecommendationsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RegenerateRecommendationsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Regenerating recommendations for ProfileId: {ProfileId}", request.ProfileId);

            // Remove existing rows for this profile
            var existing = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            _unitOfWork.PersonalizedRows.DeleteRange(existing);

            // Seed placeholder rows from top-rated shows (simple popularity approach)
            var topShows = _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false)
                .OrderByDescending(s => s.ImdbRating)
                .Take(20)
                .Select(s => s.Id)
                .ToList();

            var popularShows = _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false)
                .OrderByDescending(s => s.RottenTomatoesScore)
                .Take(20)
                .Select(s => s.Id)
                .ToList();

            var rows = new List<PersonalizedRow>
            {
                new PersonalizedRow
                {
                    ProfileId = request.ProfileId,
                    RowName = "Recommended For You",
                    ShowIdsJson = JsonSerializer.Serialize(topShows),
                    GeneratedAt = DateTime.UtcNow
                },
                new PersonalizedRow
                {
                    ProfileId = request.ProfileId,
                    RowName = "Popular Now",
                    ShowIdsJson = JsonSerializer.Serialize(popularShows),
                    GeneratedAt = DateTime.UtcNow
                }
            };

            await _unitOfWork.PersonalizedRows.AddRangeAsync(rows, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonalizedRow, object>(
                tableName: "PersonalizedRows",
                recordId: request.ProfileId,
                action: "REGENERATE",
                oldValues: null,
                newValues: new { request.ProfileId, RowCount = rows.Count },
                changedByUserId: request.ActorUserId);

            _logger.LogInformation("Recommendations regenerated for ProfileId: {ProfileId}", request.ProfileId);
            return true;
        }
    }
}
