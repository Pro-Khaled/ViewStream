using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.UpdateGenre
{
    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync<int>(request.Id, cancellationToken);
            if (genre == null) return false;

            _mapper.Map(request.Dto, genre);
            _unitOfWork.Genres.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
