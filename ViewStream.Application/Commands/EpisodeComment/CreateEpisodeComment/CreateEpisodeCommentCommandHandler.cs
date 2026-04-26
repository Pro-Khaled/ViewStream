using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;
    public class CreateEpisodeCommentCommandHandler : IRequestHandler<CreateEpisodeCommentCommand, EpisodeCommentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateEpisodeCommentCommandHandler> _logger;

        public CreateEpisodeCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateEpisodeCommentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<EpisodeCommentDto> Handle(CreateEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating comment for EpisodeId: {EpisodeId}, ProfileId: {ProfileId}",
                request.Dto.EpisodeId, request.ProfileId);

            var comment = _mapper.Map<EpisodeComment>(request.Dto);
            comment.ProfileId = request.ProfileId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.IsDeleted = false;
            comment.IsEdited = false;

            await _unitOfWork.EpisodeComments.AddAsync(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<EpisodeComment, object>(
                tableName: "EpisodeComments",
                recordId: comment.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Comment created with Id: {CommentId}", comment.Id);

            var result = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.Id == comment.Id,
                include: q => q.Include(c => c.Profile).Include(c => c.Episode),
                cancellationToken: cancellationToken);
            return _mapper.Map<EpisodeCommentDto>(result.First());
        }
    }
}
