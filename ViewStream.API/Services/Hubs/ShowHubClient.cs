using Microsoft.AspNetCore.SignalR;
using ViewStream.API.Hubs;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services.Hubs;

namespace ViewStream.API.Services.Hubs
{
    public class ShowHubClient : IShowHubClient
    {
        private readonly IHubContext<ShowHub> _hubContext;

        public ShowHubClient(IHubContext<ShowHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendShowPosterUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"show-{show.Id}").SendAsync("ShowPosterUpdated", show, cancellationToken);
        }

        public async Task SendShowBackdropUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"show-{show.Id}").SendAsync("ShowBackdropUpdated", show, cancellationToken);
        }

        public async Task SendShowTrailerUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"show-{show.Id}").SendAsync("ShowTrailerUpdated", show, cancellationToken);
        }
    }
}
