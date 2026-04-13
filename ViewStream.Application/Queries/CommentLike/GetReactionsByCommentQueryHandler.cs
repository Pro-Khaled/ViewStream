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
    public class GetReactionsByCommentQueryHandler : IRequestHandler<GetReactionsByCommentQuery, List<CommentLikeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReactionsByCommentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CommentLikeDto>> Handle(GetReactionsByCommentQuery request, CancellationToken cancellationToken)
        {
            var reactions = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.CommentId,
                include: q => q.Include(cl => cl.Profile),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<CommentLikeDto>>(reactions.OrderByDescending(r => r.CreatedAt));
        }
    }
}
