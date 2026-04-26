using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Subtitle.UploadSubtitleFile
{
    public record UploadSubtitleFileCommand(long SubtitleId, IFormFile File, long ActorUserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
