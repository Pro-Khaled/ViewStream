using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode
{
    public class GetEpisodeByIdQueryHandler : IRequestHandler<GetEpisodeByIdQuery, EpisodeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEpisodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EpisodeDto?> Handle(GetEpisodeByIdQuery request, CancellationToken cancellationToken)
        {
            var episodes = await _unitOfWork.Episodes.FindAsync(
                predicate: e => e.Id == request.Id && e.IsDeleted != true,
                include: q => q.Include(e => e.Season).ThenInclude(s => s.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var episode = episodes.FirstOrDefault();
            return episode == null ? null : _mapper.Map<EpisodeDto>(episode);
        }
    }
}
