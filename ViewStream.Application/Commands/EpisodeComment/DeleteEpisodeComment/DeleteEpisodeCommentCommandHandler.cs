using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;

    public class DeleteEpisodeCommentCommandHandler : IRequestHandler<DeleteEpisodeCommentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteEpisodeCommentCommandHandler> _logger;

        public DeleteEpisodeCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteEpisodeCommentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting comment Id: {CommentId} by ProfileId: {ProfileId}, IsAdmin: {IsAdmin}",
                request.CommentId, request.ProfileId, request.IsAdmin);

            var comment = await _unitOfWork.EpisodeComments.GetByIdAsync<long>(request.CommentId, cancellationToken);
            if (comment == null || comment.IsDeleted == true)
            {
                _logger.LogWarning("Comment not found or already deleted. Id: {CommentId}", request.CommentId);
                return false;
            }

            if (!request.IsAdmin && comment.ProfileId != request.ProfileId)
            {
                _logger.LogWarning("Access denied for deleting comment Id: {CommentId}", request.CommentId);
                return false;
            }

            var oldValues = _mapper.Map<EpisodeCommentDto>(comment);
            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            // Soft delete replies
            var replies = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.ParentCommentId == request.CommentId,
                cancellationToken: cancellationToken);
            foreach (var reply in replies)
            {
                reply.IsDeleted = true;
                reply.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<EpisodeComment, object>(
                tableName: "EpisodeComments",
                recordId: comment.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Comment soft-deleted with Id: {CommentId}", comment.Id);
            return true;
        }
    }
}
