using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subtitle.UpdateSubtitle
{
    public record UpdateSubtitleCommand(long Id, UpdateSubtitleDto Dto) : IRequest<bool>;

}
