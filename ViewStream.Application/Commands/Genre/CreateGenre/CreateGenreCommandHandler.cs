using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.CreateGenre
{
    using Genre= Domain.Entities.Genre;

    public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = _mapper.Map<Genre>(request.Dto);
            await _unitOfWork.Genres.AddAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return genre.Id;
        }
    }
}
