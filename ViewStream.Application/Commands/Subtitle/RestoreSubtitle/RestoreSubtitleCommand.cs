using MediatR;


namespace ViewStream.Application.Commands.Subtitle.RestoreSubtitle
{
    public record RestoreSubtitleCommand(long Id) : IRequest<bool>;

}
