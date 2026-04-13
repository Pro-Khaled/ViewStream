using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Season
{
    public class GetSeasonByIdQueryHandler : IRequestHandler<GetSeasonByIdQuery, SeasonDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSeasonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SeasonDto?> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
        {
            var seasons = await _unitOfWork.Seasons.FindAsync(
                predicate: s => s.Id == request.Id && s.IsDeleted != true,
                include: q => q.Include(s => s.Show).Include(s => s.Episodes.Where(e => e.IsDeleted != true)),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var season = seasons.FirstOrDefault();
            return season == null ? null : _mapper.Map<SeasonDto>(season);
        }
    }
}
