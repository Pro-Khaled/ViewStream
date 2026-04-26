using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLike
{
    using CommentLike = ViewStream.Domain.Entities.CommentLike;
    public class DeleteCommentLikeCommandHandler : IRequestHandler<DeleteCommentLikeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteCommentLikeCommandHandler> _logger;

        public DeleteCommentLikeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteCommentLikeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCommentLikeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting reaction for CommentId: {CommentId}, ProfileId: {ProfileId}",
                request.CommentId, request.ProfileId);

            var likes = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.CommentId && cl.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var like = likes.FirstOrDefault();
            if (like == null)
            {
                _logger.LogWarning("Reaction not found for CommentId: {CommentId}, ProfileId: {ProfileId}",
                    request.CommentId, request.ProfileId);
                return false;
            }

            var oldValues = _mapper.Map<CommentLikeDto>(like);
            _unitOfWork.CommentLikes.Delete(like);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<CommentLike, object>(
                tableName: "CommentLikes",
                recordId: like.CommentId,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Reaction deleted for CommentId: {CommentId}, ProfileId: {ProfileId}",
                request.CommentId, request.ProfileId);
            return true;
        }
    }
}

