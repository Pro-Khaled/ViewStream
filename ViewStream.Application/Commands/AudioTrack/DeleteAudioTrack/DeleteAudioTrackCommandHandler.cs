using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack
{
    public class DeleteAudioTrackCommandHandler : IRequestHandler<DeleteAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAudioTrackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted == true)
                return false;

            audioTrack.IsDeleted = true;
            audioTrack.DeletedAt = DateTime.UtcNow;
            audioTrack.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
