using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.RestoreEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;

    /// <summary>
    /// Handles restoring a soft-deleted episode comment for admin.
    /// </summary>
    public class RestoreEpisodeCommentCommandHandler : IRequestHandler<RestoreEpisodeCommentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreEpisodeCommentCommandHandler> _logger;

        public RestoreEpisodeCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreEpisodeCommentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        /// <summary>
        /// Restores a soft-deleted episode comment by setting IsDeleted=false and DeletedAt=null.
        /// Also updates UpdatedAt and writes an audit record.
        /// </summary>
        /// <param name="request">Restore request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if restored; otherwise false.</returns>
        public async Task<bool> Handle(RestoreEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Restoring episode comment Id: {CommentId} by admin user: {AdminUserId}",
                request.Id,
                request.RestoredByUserId);

            var comment = await _unitOfWork.EpisodeComments.GetByIdAsync<long>(request.Id, cancellationToken);
            if (comment == null || comment.IsDeleted != true)
            {
                _logger.LogWarning(
                    "Episode comment not found or not deleted. Id: {CommentId}",
                    request.Id);
                return false;
            }

            var oldValues = _mapper.Map<EpisodeCommentDto>(comment);

            comment.IsDeleted = false;
            comment.DeletedAt = null;
            comment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<EpisodeComment, object>(
                tableName: "EpisodeComments",
                recordId: comment.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.RestoredByUserId);

            _logger.LogInformation("Episode comment restored. Id: {CommentId}", comment.Id);
            return true;
        }
    }
}
