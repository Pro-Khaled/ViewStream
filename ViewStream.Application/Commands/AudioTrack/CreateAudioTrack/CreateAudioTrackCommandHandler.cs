using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.CreateAudioTrack
{
    using AudioTrack = Domain.Entities.AudioTrack;
    public class CreateAudioTrackCommandHandler : IRequestHandler<CreateAudioTrackCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAudioTrackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = _mapper.Map<AudioTrack>(request.Dto);
            audioTrack.CreatedAt = DateTime.UtcNow;
            audioTrack.IsDeleted = false;

            await _unitOfWork.AudioTracks.AddAsync(audioTrack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return audioTrack.Id;
        }
    }
}
