using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Show
{
    public class GetShowByIdQueryHandler : IRequestHandler<GetShowByIdQuery, ShowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetShowByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShowDto?> Handle(GetShowByIdQuery request, CancellationToken cancellationToken)
        {
            var shows = await _unitOfWork.Shows.FindAsync(
                predicate: s => s.Id == request.Id && s.IsDeleted != true,
                include: q => q
                    .Include(s => s.Genres)
                    .Include(s => s.Tags)
                    .Include(s => s.Seasons.Where(se => se.IsDeleted != true))
                        .ThenInclude(se => se.Episodes.Where(e => e.IsDeleted != true)),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var show = shows.FirstOrDefault();
            return show == null ? null : _mapper.Map<ShowDto>(show);
        }
    }
}
