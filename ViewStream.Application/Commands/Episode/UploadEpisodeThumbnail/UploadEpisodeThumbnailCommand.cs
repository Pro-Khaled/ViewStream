using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail
{
    public record UploadEpisodeThumbnailCommand(long EpisodeId, IFormFile ThumbnailFile, long UploadedByUserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => UploadedByUserId;
    }
}
