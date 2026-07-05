using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Controllers
{
    /// <summary>
    /// Endpoints for user appeals against moderation decisions.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/appeals")]
    [Authorize]
    public class AppealsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReputationService _reputationService;
        private readonly ILogger<AppealsController> _logger;

        public AppealsController(
            IUnitOfWork unitOfWork,
            IReputationService reputationService,
            ILogger<AppealsController> logger)
        {
            _unitOfWork = unitOfWork;
            _reputationService = reputationService;
            _logger = logger;
        }

        /// <summary>
        /// Submit an appeal against a moderation decision (hidden content or ban).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubmitAppeal([FromBody] SubmitAppealDto dto)
        {
            var userIdClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // Check for existing pending appeal
            var existingAppeals = await _unitOfWork.Repository<Appeal>().FindAsync(
                a => a.UserId == userId && a.EntityType == dto.EntityType && a.EntityId == dto.EntityId && a.Status == "pending");
            if (existingAppeals.Any())
                return BadRequest("You already have a pending appeal for this item.");

            var appeal = new Appeal
            {
                UserId = userId,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                Reason = dto.Reason,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Appeal>().AddAsync(appeal);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appeal submitted: User {UserId}, {EntityType} {EntityId}", userId, dto.EntityType, dto.EntityId);
            return Ok(new { appeal.Id, Message = "Appeal submitted successfully." });
        }

        /// <summary>
        /// Admin: approve an appeal (unhide content or unban user).
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ApproveAppeal(long id)
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            long.TryParse(userIdClaim, out var adminUserId);

            var appeals = await _unitOfWork.Repository<Appeal>().FindAsync(a => a.Id == id);
            var appeal = appeals.FirstOrDefault();
            if (appeal == null) return NotFound("Appeal not found.");
            if (appeal.Status != "pending") return BadRequest("Appeal is not pending.");

            appeal.Status = "approved";
            appeal.ReviewedAt = DateTime.UtcNow;
            appeal.ReviewedByUserId = adminUserId;
            _unitOfWork.Repository<Appeal>().Update(appeal);

            // Reverse the moderation action
            switch (appeal.EntityType.ToLowerInvariant())
            {
                case "comment":
                    var comments = await _unitOfWork.EpisodeComments.FindAsync(c => c.Id == appeal.EntityId);
                    var comment = comments.FirstOrDefault();
                    if (comment != null)
                    {
                        comment.IsHidden = false;
                        _unitOfWork.EpisodeComments.Update(comment);
                    }
                    break;

                case "episode":
                    var episodes = await _unitOfWork.Episodes.FindAsync(e => e.Id == appeal.EntityId);
                    var episode = episodes.FirstOrDefault();
                    if (episode != null)
                    {
                        episode.IsHidden = false;
                        _unitOfWork.Episodes.Update(episode);
                    }
                    break;

                case "ban":
                    // Restore reputation and unban
                    await _reputationService.AdjustReputationAsync(appeal.UserId, 25);
                    var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Id == appeal.UserId);
                    var user = users.FirstOrDefault();
                    if (user != null)
                    {
                        user.IsBannedUntil = null;
                        user.IsBlocked = false;
                        user.BlockedReason = null;
                        _unitOfWork.Repository<User>().Update(user);
                    }
                    break;
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Appeal {AppealId} approved by Admin {AdminId}", id, adminUserId);
            return Ok(new { Message = "Appeal approved. Moderation action reversed." });
        }

        /// <summary>
        /// Admin: reject an appeal.
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RejectAppeal(long id)
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            long.TryParse(userIdClaim, out var adminUserId);

            var appeals = await _unitOfWork.Repository<Appeal>().FindAsync(a => a.Id == id);
            var appeal = appeals.FirstOrDefault();
            if (appeal == null) return NotFound("Appeal not found.");
            if (appeal.Status != "pending") return BadRequest("Appeal is not pending.");

            appeal.Status = "rejected";
            appeal.ReviewedAt = DateTime.UtcNow;
            appeal.ReviewedByUserId = adminUserId;
            _unitOfWork.Repository<Appeal>().Update(appeal);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appeal {AppealId} rejected by Admin {AdminId}", id, adminUserId);
            return Ok(new { Message = "Appeal rejected." });
        }
    }

    public class SubmitAppealDto
    {
        /// <summary>"Comment", "Episode", or "Ban"</summary>
        public string EntityType { get; set; } = string.Empty;
        public long EntityId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
