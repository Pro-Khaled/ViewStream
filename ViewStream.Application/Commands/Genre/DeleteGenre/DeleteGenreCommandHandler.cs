using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.DeleteGenre
{
    public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenreCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync<int>(request.Id, cancellationToken);
            if (genre == null) return false;

            _unitOfWork.Genres.Delete(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
