using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment
{
    public class DeleteEpisodeCommentCommandHandler : IRequestHandler<DeleteEpisodeCommentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEpisodeCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EpisodeComments.GetByIdAsync<long>(request.Id, cancellationToken);
            if (comment == null || comment.IsDeleted == true)
                return false;

            // Check permission: owner or admin
            if (!request.IsAdmin && comment.ProfileId != request.ProfileId)
                return false;

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            // Soft delete replies as well
            var replies = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.ParentCommentId == request.Id,
                cancellationToken: cancellationToken);

            foreach (var reply in replies)
            {
                reply.IsDeleted = true;
                reply.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
