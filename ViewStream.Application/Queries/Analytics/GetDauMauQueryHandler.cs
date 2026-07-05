using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>
    /// Computes DAU (Daily Active Users) and MAU (Monthly Active Users) from PlaybackEvents.
    /// DAU/MAU ratio indicates user engagement (healthy: >20%).
    /// </summary>
    public class GetDauMauQueryHandler : IRequestHandler<GetDauMauQuery, List<DauMauDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDauMauQueryHandler> _logger;

        public GetDauMauQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDauMauQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<DauMauDto>> Handle(GetDauMauQuery request, CancellationToken cancellationToken)
        {
            var fromDate = request.From.ToDateTime(TimeOnly.MinValue);
            var toDate = request.To.ToDateTime(TimeOnly.MaxValue);

            var events = await _unitOfWork.PlaybackEvents.GetQueryable()
                .Where(pe => pe.CreatedAt >= fromDate && pe.CreatedAt <= toDate)
                .Select(pe => new { pe.ProfileId, pe.CreatedAt })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var results = new List<DauMauDto>();

            for (var date = request.From; date <= request.To; date = date.AddDays(1))
            {
                var dayStart = date.ToDateTime(TimeOnly.MinValue);
                var dayEnd = date.ToDateTime(TimeOnly.MaxValue);
                var monthStart = new DateOnly(date.Year, date.Month, 1).ToDateTime(TimeOnly.MinValue);

                var dau = events
                    .Where(e => e.CreatedAt >= dayStart && e.CreatedAt <= dayEnd)
                    .Select(e => e.ProfileId)
                    .Distinct()
                    .Count();

                var mau = events
                    .Where(e => e.CreatedAt >= monthStart && e.CreatedAt <= dayEnd)
                    .Select(e => e.ProfileId)
                    .Distinct()
                    .Count();

                results.Add(new DauMauDto
                {
                    Date = date,
                    DailyActiveUsers = dau,
                    MonthlyActiveUsers = mau,
                    DauMauRatio = mau > 0 ? Math.Round((decimal)dau / mau, 4) : 0
                });
            }

            return results;
        }
    }
}
