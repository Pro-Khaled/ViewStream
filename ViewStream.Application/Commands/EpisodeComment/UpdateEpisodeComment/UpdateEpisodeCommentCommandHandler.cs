using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;
    public class UpdateEpisodeCommentCommandHandler : IRequestHandler<UpdateEpisodeCommentCommand, EpisodeCommentDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateEpisodeCommentCommandHandler> _logger;

        public UpdateEpisodeCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateEpisodeCommentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<EpisodeCommentDto?> Handle(UpdateEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating comment Id: {CommentId} by ProfileId: {ProfileId}",
                request.CommentId, request.ProfileId);

            var comment = await _unitOfWork.EpisodeComments.GetByIdAsync<long>(request.CommentId, cancellationToken);
            if (comment == null || comment.ProfileId != request.ProfileId || comment.IsDeleted == true)
            {
                _logger.LogWarning("Comment not found, already deleted, or access denied. Id: {CommentId}", request.CommentId);
                return null;
            }

            var oldValues = _mapper.Map<EpisodeCommentDto>(comment);
            comment.CommentText = request.Dto.CommentText;
            comment.IsEdited = true;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.EpisodeComments.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<EpisodeComment, object>(
                tableName: "EpisodeComments",
                recordId: comment.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Comment updated with Id: {CommentId}", comment.Id);

            var result = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.Id == comment.Id,
                include: q => q.Include(c => c.Profile).Include(c => c.Episode),
                cancellationToken: cancellationToken);
            return _mapper.Map<EpisodeCommentDto>(result.First());
        }
    }
}
