using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;
    public class CreateEpisodeCommentCommandHandler : IRequestHandler<CreateEpisodeCommentCommand, EpisodeCommentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateEpisodeCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EpisodeCommentDto> Handle(CreateEpisodeCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = _mapper.Map<EpisodeComment>(request.Dto);
            comment.ProfileId = request.ProfileId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.IsDeleted = false;
            comment.IsEdited = false;

            await _unitOfWork.EpisodeComments.AddAsync(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load navigation properties for DTO
            var result = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.Id == comment.Id,
                include: q => q.Include(c => c.Profile).Include(c => c.Episode),
                cancellationToken: cancellationToken);

            return _mapper.Map<EpisodeCommentDto>(result.First());
        }
    }
}
