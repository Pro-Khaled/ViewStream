using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for managing GDPR data deletion requests.
    /// </summary>
    [ApiController]
    [Route("api/admin/deletion-requests")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DataDeletionAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DataDeletionAdminController> _logger;

        public DataDeletionAdminController(IUnitOfWork unitOfWork, ILogger<DataDeletionAdminController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Gets all pending data deletion requests.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _unitOfWork.DataDeletionRequests.FindAsync(
                r => r.Status == "pending",
                include: q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                    .Include(q, r => r.User),
                asNoTracking: true);

            return Ok(requests.Select(r => new
            {
                r.Id,
                r.UserId,
                UserEmail = r.User?.Email,
                r.Reason,
                r.RequestedAt,
                r.Status
            }));
        }

        /// <summary>
        /// Approves a data deletion request. The DataDeletionProcessingJob will process it.
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(long id)
        {
            var requests = await _unitOfWork.DataDeletionRequests.FindAsync(r => r.Id == id);
            var request = requests.FirstOrDefault();
            if (request == null) return NotFound("Deletion request not found.");
            if (request.Status != "pending") return BadRequest("Request is not in pending status.");

            request.Status = "approved";
            request.ReviewedAt = DateTime.UtcNow;
            _unitOfWork.DataDeletionRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deletion request {RequestId} approved.", id);
            return Ok(new { Message = "Deletion request approved. Will be processed by the next job run." });
        }

        /// <summary>
        /// Rejects a data deletion request with a reason.
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(long id, [FromBody] RejectDeletionDto dto)
        {
            var requests = await _unitOfWork.DataDeletionRequests.FindAsync(r => r.Id == id);
            var request = requests.FirstOrDefault();
            if (request == null) return NotFound("Deletion request not found.");
            if (request.Status != "pending") return BadRequest("Request is not in pending status.");

            request.Status = "rejected";
            request.ReviewedAt = DateTime.UtcNow;
            request.RejectionReason = dto.Reason;
            _unitOfWork.DataDeletionRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deletion request {RequestId} rejected: {Reason}", id, dto.Reason);
            return Ok(new { Message = "Deletion request rejected." });
        }
    }

    public class RejectDeletionDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
