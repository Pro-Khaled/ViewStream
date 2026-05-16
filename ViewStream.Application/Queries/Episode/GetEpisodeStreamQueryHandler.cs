using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode
{
    public class GetEpisodeStreamQueryHandler : IRequestHandler<GetEpisodeStreamQuery, EpisodeStreamUrlDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEpisodeStreamQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EpisodeStreamUrlDto?> Handle(GetEpisodeStreamQuery request, CancellationToken cancellationToken)
        {
            var episode = await _unitOfWork.Episodes.GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.IsDeleted != true, cancellationToken);

            if (episode == null) return null;

            return new EpisodeStreamUrlDto
            {
                VideoUrl = episode.VideoUrl
            };
        }
    }
}
