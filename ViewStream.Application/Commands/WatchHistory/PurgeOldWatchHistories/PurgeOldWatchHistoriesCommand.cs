using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using MediatR;

namespace ViewStream.Application.Commands.WatchHistory.PurgeOldWatchHistories
{
    public record PurgeOldWatchHistoriesCommand(int DaysToKeep, long AdminUserId) : IRequest<int>;
}
