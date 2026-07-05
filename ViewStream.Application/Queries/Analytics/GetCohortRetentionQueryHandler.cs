using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>
    /// Computes month-over-month cohort retention.
    /// A cohort = users whose first playback event occurred in a given month.
    /// Retention = % of cohort users active in subsequent months.
    /// </summary>
    public class GetCohortRetentionQueryHandler : IRequestHandler<GetCohortRetentionQuery, List<CohortRetentionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCohortRetentionQueryHandler> _logger;

        public GetCohortRetentionQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCohortRetentionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CohortRetentionDto>> Handle(GetCohortRetentionQuery request, CancellationToken cancellationToken)
        {
            var allEvents = await _unitOfWork.PlaybackEvents.GetQueryable()
                .Where(pe => pe.CreatedAt >= DateTime.UtcNow.AddMonths(-request.CohortMonths - 3))
                .Select(pe => new { pe.ProfileId, pe.CreatedAt })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Group by profile to find first activity month (cohort assignment)
            var profileFirstMonth = allEvents
                .GroupBy(e => e.ProfileId)
                .ToDictionary(
                    g => g.Key,
                    g => new DateOnly(g.Min(e => e.CreatedAt!.Value.Year), g.Min(e => e.CreatedAt!.Value.Month), 1));

            // Group all events by (profile, month)
            var profileMonthActivity = allEvents
                .GroupBy(e => (e.ProfileId, Month: new DateOnly(e.CreatedAt!.Value.Year, e.CreatedAt!.Value.Month, 1)))
                .Select(g => g.Key)
                .ToHashSet();

            var results = new List<CohortRetentionDto>();
            var startMonth = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-request.CohortMonths));

            for (int i = 0; i < request.CohortMonths; i++)
            {
                var cohortMonth = startMonth.AddMonths(i);
                var cohortMonth2 = new DateOnly(cohortMonth.Year, cohortMonth.Month, 1);

                var cohortUsers = profileFirstMonth
                    .Where(kvp => kvp.Value == cohortMonth2)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (cohortUsers.Count == 0) continue;

                var retention = new CohortRetentionDto
                {
                    CohortMonth = cohortMonth2.ToString("yyyy-MM"),
                    TotalUsers = cohortUsers.Count,
                    RetentionData = new List<RetentionDataPoint>()
                };

                // Check retention for subsequent months
                var maxOffset = Math.Min(6, (int)((DateTime.UtcNow.Year * 12 + DateTime.UtcNow.Month)
                    - (cohortMonth2.Year * 12 + cohortMonth2.Month)));

                for (int offset = 0; offset <= maxOffset; offset++)
                {
                    var checkMonth = cohortMonth2.AddMonths(offset);
                    var activeInMonth = cohortUsers.Count(uid =>
                        profileMonthActivity.Contains((uid, checkMonth)));

                    retention.RetentionData.Add(new RetentionDataPoint
                    {
                        MonthOffset = offset,
                        ActiveUsers = activeInMonth,
                        RetentionRate = Math.Round((decimal)activeInMonth / cohortUsers.Count * 100, 2)
                    });
                }

                results.Add(retention);
            }

            return results;
        }
    }
}
