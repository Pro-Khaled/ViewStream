using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack
{
    public class RestoreAudioTrackCommandHandler : IRequestHandler<RestoreAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreAudioTrackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted != true)
                return false;

            audioTrack.IsDeleted = false;
            audioTrack.DeletedAt = null;
            audioTrack.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
