using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.AudioTrack.UploadAudioFile
{
    public record UploadAudioFileCommand(long AudioTrackId, IFormFile File, long UploadedByUserId)
    : IRequest<string>, IHasUserId
    {
        public long? UserId => UploadedByUserId;
    }
}

