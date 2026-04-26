using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.CreateCommentLike
{
    using CommentLike = ViewStream.Domain.Entities.CommentLike;
    public class UpsertCommentLikeCommandHandler : IRequestHandler<UpsertCommentLikeCommand, CommentLikeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertCommentLikeCommandHandler> _logger;

        public UpsertCommentLikeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpsertCommentLikeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<CommentLikeDto> Handle(UpsertCommentLikeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting reaction for CommentId: {CommentId}, ProfileId: {ProfileId}",
                request.Dto.CommentId, request.ProfileId);

            var existing = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.Dto.CommentId && cl.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var like = existing.FirstOrDefault();
            string action;
            object? oldValues = null;

            if (like == null)
            {
                action = "INSERT";
                like = new CommentLike
                {
                    CommentId = request.Dto.CommentId,
                    ProfileId = request.ProfileId,
                    ReactionType = request.Dto.ReactionType ?? "like",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.CommentLikes.AddAsync(like, cancellationToken);
            }
            else
            {
                action = "UPDATE";
                oldValues = new { like.ReactionType };
                like.ReactionType = request.Dto.ReactionType ?? "like";
                like.CreatedAt = DateTime.UtcNow;
                _unitOfWork.CommentLikes.Update(like);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<CommentLike, object>(
                tableName: "CommentLikes",
                recordId: like.CommentId,           // Composite PK: we use CommentId as primary identifier
                action: action,
                oldValues: oldValues,
                newValues: new { like.ProfileId, like.ReactionType },
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Reaction upserted for CommentId: {CommentId}, ProfileId: {ProfileId}",
                like.CommentId, like.ProfileId);

            var result = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == like.CommentId && cl.ProfileId == like.ProfileId,
                include: q => q.Include(cl => cl.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<CommentLikeDto>(result.First());
        }
    }
}

