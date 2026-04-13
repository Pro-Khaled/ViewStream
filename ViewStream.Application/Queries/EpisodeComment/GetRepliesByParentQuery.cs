using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.EpisodeComment
{
    // Get replies for a specific comment

    public record GetRepliesByParentQuery(long ParentCommentId) : IRequest<List<EpisodeCommentListItemDto>>;

}
