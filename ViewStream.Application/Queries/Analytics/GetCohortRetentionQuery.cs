using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>Query for month-over-month cohort retention analysis.</summary>
    public record GetCohortRetentionQuery(int CohortMonths = 6) : IRequest<List<CohortRetentionDto>>;
}
