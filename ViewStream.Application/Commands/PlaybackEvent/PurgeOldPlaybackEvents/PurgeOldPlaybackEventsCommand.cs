using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using MediatR;

namespace ViewStream.Application.Commands.PlaybackEvent.PurgeOldPlaybackEvents
{
    public record PurgeOldPlaybackEventsCommand(int DaysToKeep, long AdminUserId) : IRequest<int>;
}
