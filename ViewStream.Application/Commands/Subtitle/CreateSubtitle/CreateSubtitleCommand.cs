using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subtitle.CreateSubtitle
{
    public record CreateSubtitleCommand(CreateSubtitleDto Dto) : IRequest<long>;

}
