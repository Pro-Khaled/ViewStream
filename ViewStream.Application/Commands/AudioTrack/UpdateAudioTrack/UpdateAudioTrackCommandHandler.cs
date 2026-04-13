using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack
{
    //    public class UpdateAudioTrackCommandHandler : IRequestHandler<UpdateAudioTrackCommand, BaseResponse<AudioTrackDto>>
    public class UpdateAudioTrackCommandHandler : IRequestHandler<UpdateAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAudioTrackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted == true)
                return false;

            _mapper.Map(request.Dto, audioTrack);
            audioTrack.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AudioTracks.Update(audioTrack);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
