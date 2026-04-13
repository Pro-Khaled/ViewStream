using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AudioTrack
{
    public class GetAudioTracksByEpisodeQueryHandler : IRequestHandler<GetAudioTracksByEpisodeQuery, List<AudioTrackListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAudioTracksByEpisodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AudioTrackListItemDto>> Handle(GetAudioTracksByEpisodeQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.AudioTracks.GetQueryable()
                .Where(a => a.EpisodeId == request.EpisodeId);

            if (!request.IncludeDeleted)
                query = query.Where(a => a.IsDeleted != true);

            var audioTracks = await query
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.LanguageCode)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<AudioTrackListItemDto>>(audioTracks);
        }
    }
}
