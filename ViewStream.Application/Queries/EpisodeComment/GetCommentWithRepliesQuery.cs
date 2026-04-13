using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.EpisodeComment
{
    // Get single comment with replies tree (limited depth)
    public record GetCommentWithRepliesQuery(long Id, int MaxDepth = 3) : IRequest<EpisodeCommentDto?>;
}
