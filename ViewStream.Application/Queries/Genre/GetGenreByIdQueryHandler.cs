using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Genre
{
    public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, GenreDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetGenreByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenreDto?> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
        {
            var genres = await _unitOfWork.Genres.FindAsync(
                predicate: g => g.Id == request.Id,
                include: q => q.Include(g => g.Shows),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var genre = genres.FirstOrDefault();
            return genre == null ? null : _mapper.Map<GenreDto>(genre);
        }
    }
}
