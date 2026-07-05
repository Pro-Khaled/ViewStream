namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Checks block status between users for social integrity enforcement.
    /// </summary>
    public interface IBlockCheckService
    {
        /// <summary>Checks if either user has blocked the other.</summary>
        Task<bool> IsBlockedAsync(long userId, long targetUserId);

        /// <summary>Returns all user IDs blocked by the given user.</summary>
        Task<List<long>> GetBlockedUserIdsAsync(long userId);
    }
}
