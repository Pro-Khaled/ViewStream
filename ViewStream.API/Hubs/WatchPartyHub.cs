using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.API.Hubs
{
    /// <summary>
    /// SignalR hub for synchronized watch party playback.
    /// Manages join/leave, host playback controls (play/pause/seek),
    /// chat messaging, automatic host promotion on disconnect,
    /// and latency-compensated playback synchronization.
    /// </summary>
    [Authorize]
    public class WatchPartyHub : Hub
    {
        private readonly IWatchPartyStateService _stateService;
        private readonly ILogger<WatchPartyHub> _logger;

        public WatchPartyHub(IWatchPartyStateService stateService, ILogger<WatchPartyHub> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        public async Task JoinParty(string partyCode, long profileId, long userId)
        {
            _logger.LogInformation("Profile {ProfileId} joining party {PartyCode}", profileId, partyCode);

            var state = await _stateService.GetStateAsync(partyCode);
            if (state == null)
            {
                await Clients.Caller.SendAsync("Error", "Watch party not found or has ended.");
                return;
            }

            // Add participant to state
            state.Participants.Add(new ParticipantInfo
            {
                ProfileId = profileId,
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                JoinedAtUtc = DateTime.UtcNow
            });
            state.LastActivityUtc = DateTime.UtcNow;
            await _stateService.SetStateAsync(partyCode, state);

            // Track connection-to-party mapping for host failover
            await _stateService.TrackConnectionAsync(Context.ConnectionId, partyCode);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"watchparty-{partyCode}");
            await Clients.Group($"watchparty-{partyCode}").SendAsync("ParticipantJoined", profileId);

            // Send current state to the new participant
            await Clients.Caller.SendAsync("SyncState", new
            {
                state.CurrentPositionSeconds,
                state.IsPlaying,
                state.HostProfileId,
                state.BaseTimestampMs,
                ParticipantCount = state.Participants.Count
            });
        }

        public async Task LeaveParty(string partyCode, long profileId)
        {
            _logger.LogInformation("Profile {ProfileId} leaving party {PartyCode}", profileId, partyCode);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"watchparty-{partyCode}");

            var state = await _stateService.GetStateAsync(partyCode);
            if (state != null)
            {
                state.Participants.RemoveAll(p => p.ConnectionId == Context.ConnectionId);
                state.LastActivityUtc = DateTime.UtcNow;
                await _stateService.SetStateAsync(partyCode, state);
            }

            // Untrack connection-to-party mapping
            await _stateService.UntrackConnectionAsync(Context.ConnectionId, partyCode);

            await Clients.Group($"watchparty-{partyCode}").SendAsync("ParticipantLeft", profileId);
        }

        public async Task HostPlay(string partyCode)
        {
            var state = await _stateService.GetStateAsync(partyCode);
            if (state == null || Context.ConnectionId != state.HostConnectionId) return;

            state.IsPlaying = true;
            state.LastActivityUtc = DateTime.UtcNow;
            state.BaseTimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            await _stateService.SetStateAsync(partyCode, state);

            await Clients.Group($"watchparty-{partyCode}").SendAsync("Play", state.CurrentPositionSeconds);
        }

        public async Task HostPause(string partyCode)
        {
            var state = await _stateService.GetStateAsync(partyCode);
            if (state == null || Context.ConnectionId != state.HostConnectionId) return;

            state.IsPlaying = false;
            state.LastActivityUtc = DateTime.UtcNow;
            await _stateService.SetStateAsync(partyCode, state);

            await Clients.Group($"watchparty-{partyCode}").SendAsync("Pause", state.CurrentPositionSeconds);
        }

        public async Task HostSeek(string partyCode, int positionSeconds)
        {
            var state = await _stateService.GetStateAsync(partyCode);
            if (state == null || Context.ConnectionId != state.HostConnectionId) return;

            state.CurrentPositionSeconds = positionSeconds;
            state.LastActivityUtc = DateTime.UtcNow;
            state.BaseTimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            await _stateService.SetStateAsync(partyCode, state);

            await Clients.Group($"watchparty-{partyCode}").SendAsync("Seek", positionSeconds);
        }

        /// <summary>
        /// Latency-compensated playback sync. The host sends its current position and local timestamp.
        /// Other clients receive both values plus the server timestamp, allowing them to compute
        /// the network delay and adjust their position to stay within ~500ms of each other.
        /// </summary>
        public async Task SyncPlayback(string partyCode, int positionSeconds, long hostTimestampMs)
        {
            var state = await _stateService.GetStateAsync(partyCode);
            if (state == null || Context.ConnectionId != state.HostConnectionId) return;

            state.CurrentPositionSeconds = positionSeconds;
            state.BaseTimestampMs = hostTimestampMs;
            state.LastActivityUtc = DateTime.UtcNow;
            await _stateService.SetStateAsync(partyCode, state);

            var serverTimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await Clients.OthersInGroup($"watchparty-{partyCode}")
                .SendAsync("SyncPlayback", new
                {
                    PositionSeconds = positionSeconds,
                    HostTimestampMs = hostTimestampMs,
                    ServerTimestampMs = serverTimestampMs
                });
        }

        public async Task SendChatMessage(string partyCode, string message, long profileId, string profileName)
        {
            if (string.IsNullOrWhiteSpace(message) || message.Length > 500) return;

            await Clients.Group($"watchparty-{partyCode}").SendAsync("ChatMessage", new
            {
                ProfileId = profileId,
                ProfileName = profileName,
                Message = message,
                SentAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Handles disconnection: removes participant from all parties they belong to.
        /// If the disconnected user was the host, promotes the next participant (by JoinedAtUtc)
        /// and broadcasts a HostChanged event.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Connection {ConnectionId} disconnected from watch party hub", Context.ConnectionId);

            var partyCodes = await _stateService.GetPartyCodesForConnectionAsync(Context.ConnectionId);

            foreach (var partyCode in partyCodes)
            {
                var state = await _stateService.GetStateAsync(partyCode);
                if (state == null) continue;

                // Find the disconnecting participant
                var disconnectedParticipant = state.Participants
                    .FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                // Remove disconnected participant
                state.Participants.RemoveAll(p => p.ConnectionId == Context.ConnectionId);
                state.LastActivityUtc = DateTime.UtcNow;

                // Check if the disconnected user was the host
                if (Context.ConnectionId == state.HostConnectionId && state.Participants.Count > 0)
                {
                    // Promote the next participant by JoinedAtUtc (earliest joiner becomes host)
                    var newHost = state.Participants
                        .OrderBy(p => p.JoinedAtUtc)
                        .First();

                    state.HostConnectionId = newHost.ConnectionId;
                    state.HostProfileId = newHost.ProfileId;

                    _logger.LogInformation(
                        "Host failover in party {PartyCode}: promoted Profile {NewHostId} (Connection: {ConnId})",
                        partyCode, newHost.ProfileId, newHost.ConnectionId);

                    await _stateService.SetStateAsync(partyCode, state);

                    // Notify all participants about the host change
                    await Clients.Group($"watchparty-{partyCode}").SendAsync("HostChanged", new
                    {
                        NewHostProfileId = newHost.ProfileId,
                        newHost.ConnectionId,
                        ParticipantCount = state.Participants.Count
                    });
                }
                else
                {
                    await _stateService.SetStateAsync(partyCode, state);
                }

                // Notify about participant departure
                if (disconnectedParticipant != null)
                {
                    await Clients.Group($"watchparty-{partyCode}")
                        .SendAsync("ParticipantLeft", disconnectedParticipant.ProfileId);
                }

                // Remove from SignalR group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"watchparty-{partyCode}");

                // Untrack the connection
                await _stateService.UntrackConnectionAsync(Context.ConnectionId, partyCode);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
