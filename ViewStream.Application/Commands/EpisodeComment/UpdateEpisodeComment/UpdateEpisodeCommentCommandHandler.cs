using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment
{
    public class UpdateEpisodeCommentCommandHandler : IRequestHandler<UpdateEpisodeCommentCommand, EpisodeCommentDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEpisodeCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EpisodeCommentDto?> Handle(UpdateEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EpisodeComments.GetByIdAsync<long>(request.Id, cancellationToken);
            if (comment == null || comment.ProfileId != request.ProfileId || comment.IsDeleted == true)
                return null;

            comment.CommentText = request.Dto.CommentText;
            comment.IsEdited = true;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.EpisodeComments.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.Id == comment.Id,
                include: q => q.Include(c => c.Profile).Include(c => c.Episode),
                cancellationToken: cancellationToken);

            return _mapper.Map<EpisodeCommentDto>(result.First());
        }
    }
}
