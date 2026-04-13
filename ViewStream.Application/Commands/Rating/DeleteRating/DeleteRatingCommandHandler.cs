using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.DeleteRating
{
    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRatingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == request.ProfileId && r.ShowId == request.ShowId,
                cancellationToken: cancellationToken);

            var rating = ratings.FirstOrDefault();
            if (rating == null) return false;

            _unitOfWork.Ratings.Delete(rating);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
