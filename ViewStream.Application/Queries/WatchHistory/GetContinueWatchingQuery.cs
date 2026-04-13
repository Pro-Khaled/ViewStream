using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.WatchHistory
{
    public record GetContinueWatchingQuery(long ProfileId, int Limit = 10) : IRequest<List<WatchHistoryListItemDto>>;

}
