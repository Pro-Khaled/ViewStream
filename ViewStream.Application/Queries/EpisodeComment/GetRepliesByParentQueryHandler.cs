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

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetRepliesByParentQueryHandler : IRequestHandler<GetRepliesByParentQuery, List<EpisodeCommentListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRepliesByParentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<EpisodeCommentListItemDto>> Handle(GetRepliesByParentQuery request, CancellationToken cancellationToken)
        {
            var replies = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.ParentCommentId == request.ParentCommentId && c.IsDeleted != true,
                include: q => q.Include(c => c.Profile)
                               .Include(c => c.CommentLikes)
                               .Include(c => c.InverseParentComment.Where(r => r.IsDeleted != true)),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<EpisodeCommentListItemDto>>(replies.OrderBy(c => c.CreatedAt));
        }
    }
}
