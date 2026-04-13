using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserInteraction
{
    public record GetInteractionsByShowQuery(long ShowId, int Page = 1, int PageSize = 50) : IRequest<PagedResult<UserInteractionListItemDto>>;

}
