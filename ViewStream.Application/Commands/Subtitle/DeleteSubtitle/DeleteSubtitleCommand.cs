using MediatR;

namespace ViewStream.Application.Commands.Subtitle.DeleteSubtitle
{
    public record DeleteSubtitleCommand(long Id) : IRequest<bool>;       // Soft delete

}
