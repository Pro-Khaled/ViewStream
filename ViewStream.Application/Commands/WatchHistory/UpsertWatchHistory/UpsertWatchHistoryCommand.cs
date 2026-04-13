using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory
{
    public record UpsertWatchHistoryCommand(long ProfileId, CreateUpdateWatchHistoryDto Dto) : IRequest<WatchHistoryDto>;
}
