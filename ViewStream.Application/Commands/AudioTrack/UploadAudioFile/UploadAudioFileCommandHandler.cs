using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.AudioTrack;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.UploadAudioFile
{
    public class UploadAudioFileCommandHandler : IRequestHandler<UploadAudioFileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;

        public UploadAudioFileCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IEpisodeHubClient hubClient)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
        }

        public async Task<string> Handle(UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.AudioTrackId, cancellationToken);
            if (audioTrack == null)
                throw new InvalidOperationException("Audio track not found.");

            if (!string.IsNullOrEmpty(audioTrack.AudioUrl))
                _fileStorage.DeleteFile(audioTrack.AudioUrl);

            var fileUrl = await _fileStorage.SaveAudioFileAsync(request.File, request.AudioTrackId, cancellationToken);
            audioTrack.AudioUrl = fileUrl;
            audioTrack.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AudioTracks.Update(audioTrack);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedAudioDto = await _mediator.Send(new GetAudioTrackByIdQuery(request.AudioTrackId), cancellationToken);
            await _hubClient.SendAudioTrackFileUpdatedAsync(updatedAudioDto, cancellationToken);
            return fileUrl;
        }
    }
}
