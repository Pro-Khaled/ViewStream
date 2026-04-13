using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentLike
{
    public class GetUserReactionForCommentQueryHandler : IRequestHandler<GetUserReactionForCommentQuery, CommentLikeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserReactionForCommentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentLikeDto?> Handle(GetUserReactionForCommentQuery request, CancellationToken cancellationToken)
        {
            var reactions = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.CommentId && cl.ProfileId == request.ProfileId,
                include: q => q.Include(cl => cl.Profile),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var reaction = reactions.FirstOrDefault();
            return reaction == null ? null : _mapper.Map<CommentLikeDto>(reaction);
        }
    }
}
