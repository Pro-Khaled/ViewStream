using Microsoft.AspNetCore.SignalR;

namespace ViewStream.Api.Hubs;

public class EpisodeHub : Hub
{
    public async Task JoinEpisodeGroup(long episodeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"episode-{episodeId}");
    }

    public async Task LeaveEpisodeGroup(long episodeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"episode-{episodeId}");
    }
}