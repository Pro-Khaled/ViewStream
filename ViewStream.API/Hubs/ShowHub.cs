using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ViewStream.API.Hubs
{
    public class ShowHub : Hub
    {
        public async Task JoinShowGroup(long showId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"show-{showId}");
        }

        public async Task LeaveShowGroup(long showId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"show-{showId}");
        }
    }
}
