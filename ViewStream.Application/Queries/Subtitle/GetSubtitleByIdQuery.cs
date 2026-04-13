using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Subtitle
{
    public record GetSubtitleByIdQuery(long Id) : IRequest<SubtitleDto?>;

}
