using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AudioTrack
{
    public class GetAudioTrackByIdQueryHandler : IRequestHandler<GetAudioTrackByIdQuery, AudioTrackDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAudioTrackByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AudioTrackDto?> Handle(GetAudioTrackByIdQuery request, CancellationToken cancellationToken)
        {
            var audioTracks = await _unitOfWork.AudioTracks.FindAsync(
                predicate: a => a.Id == request.Id && a.IsDeleted != true,
                include: q => q.Include(a => a.Episode),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var audioTrack = audioTracks.FirstOrDefault();
            return audioTrack == null ? null : _mapper.Map<AudioTrackDto>(audioTrack);
        }
    }
}
