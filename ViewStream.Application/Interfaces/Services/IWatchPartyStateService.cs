namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Manages watch party state (current playback position, host, participants) in an external store.
    /// </summary>
    public interface IWatchPartyStateService
    {
        Task<WatchPartyState?> GetStateAsync(string partyCode);
        Task SetStateAsync(string partyCode, WatchPartyState state);
        Task RemoveStateAsync(string partyCode);

        /// <summary>
        /// Tracks the mapping from a SignalR connection to the party codes it belongs to.
        /// Used for host failover on disconnect.
        /// </summary>
        Task TrackConnectionAsync(string connectionId, string partyCode);

        /// <summary>
        /// Removes a connection's tracking entry for a specific party.
        /// </summary>
        Task UntrackConnectionAsync(string connectionId, string partyCode);

        /// <summary>
        /// Gets all party codes that a connection is currently tracked in.
        /// </summary>
        Task<List<string>> GetPartyCodesForConnectionAsync(string connectionId);
    }

    public class WatchPartyState
    {
        public string PartyCode { get; set; } = string.Empty;
        public long EpisodeId { get; set; }
        public string HostConnectionId { get; set; } = string.Empty;
        public long HostProfileId { get; set; }
        public int CurrentPositionSeconds { get; set; }
        public bool IsPlaying { get; set; }
        public DateTime LastActivityUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UTC millisecond timestamp of the host's clock when the position was last synced.
        /// Used for latency compensation: clients compare this against their own clock
        /// to determine how far ahead/behind they are.
        /// </summary>
        public long BaseTimestampMs { get; set; }

        public List<ParticipantInfo> Participants { get; set; } = new();
    }

    public class ParticipantInfo
    {
        public long ProfileId { get; set; }
        public long UserId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
