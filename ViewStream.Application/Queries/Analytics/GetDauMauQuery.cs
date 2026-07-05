using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>Query for DAU/MAU engagement metrics over a date range.</summary>
    public record GetDauMauQuery(DateOnly From, DateOnly To) : IRequest<List<DauMauDto>>;
}
