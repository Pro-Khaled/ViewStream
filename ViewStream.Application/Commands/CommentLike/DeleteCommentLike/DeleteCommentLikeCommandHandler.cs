using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLike
{
    public class DeleteCommentLikeCommandHandler : IRequestHandler<DeleteCommentLikeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentLikeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCommentLikeCommand request, CancellationToken cancellationToken)
        {
            var likes = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.CommentId && cl.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var like = likes.FirstOrDefault();
            if (like == null) return false;

            _unitOfWork.CommentLikes.Delete(like);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
