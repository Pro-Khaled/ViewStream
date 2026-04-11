using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Show
{
    public record GetShowByIdQuery(long Id) : IRequest<ShowDto?>;

}
