using MediatR;
using Microsoft.AspNetCore.Http;


namespace ViewStream.Application.Commands.AudioTrack.UploadAudioFile
{
    public record UploadAudioFileCommand(long AudioTrackId, IFormFile File, long UploadedByUserId) : IRequest<string>;
}
