using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.CreateCommentLike
{
    using CommentLike = ViewStream.Domain.Entities.CommentLike;
    public class UpsertCommentLikeCommandHandler : IRequestHandler<UpsertCommentLikeCommand, CommentLikeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertCommentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentLikeDto> Handle(UpsertCommentLikeCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.Dto.CommentId && cl.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var like = existing.FirstOrDefault();

            if (like == null)
            {
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
                like.ReactionType = request.Dto.ReactionType ?? "like";
                like.CreatedAt = DateTime.UtcNow;
                _unitOfWork.CommentLikes.Update(like);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == like.CommentId && cl.ProfileId == like.ProfileId,
                include: q => q.Include(cl => cl.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<CommentLikeDto>(result.First());
        }
    }
}
