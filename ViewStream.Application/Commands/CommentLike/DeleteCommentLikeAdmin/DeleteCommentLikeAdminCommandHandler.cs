using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLikeAdmin
{
    public class DeleteCommentLikeAdminCommandHandler : IRequestHandler<DeleteCommentLikeAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteCommentLikeAdminCommandHandler> _logger;

        public DeleteCommentLikeAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteCommentLikeAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCommentLikeAdminCommand request, CancellationToken cancellationToken)
        {
            var like = await _unitOfWork.CommentLikes.GetQueryable()
                .FirstOrDefaultAsync(x => x.CommentId == request.CommentId && x.ProfileId == request.ProfileId, cancellationToken);
            
            if (like == null)
            {
                throw new NotFoundException("CommentLike", $"{request.CommentId}/{request.ProfileId}");
            }

            _unitOfWork.CommentLikes.Delete(like);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment like removed by admin. CommentId: {CommentId}, ProfileId: {ProfileId}", request.CommentId, request.ProfileId);
            return true;
        }
    }
}

