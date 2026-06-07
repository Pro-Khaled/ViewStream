using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Contracts;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Controllers
{
    [ApiController]
    [Route("api/admin/jobs")]
    [Authorize(Roles = "SuperAdmin,ContentManager")]
    public class AdminJobsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;

        public AdminJobsController(IUnitOfWork unitOfWork, IMessageBus messageBus)
        {
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await _unitOfWork.VideoProcessingJobs.GetAllAsync();
            return Ok(jobs.Select(j => new
            {
                j.Id,
                j.EpisodeId,
                j.Status,
                j.ErrorMessage,
                j.CreatedAt,
                j.UpdatedAt
            }));
        }

        [HttpPost("{jobId}/retry")]
        public async Task<IActionResult> Retry(long jobId)
        {
            var job = await _unitOfWork.VideoProcessingJobs.GetByIdAsync(jobId);
            if (job == null) return NotFound();
            if (job.Status != "Failed") return BadRequest("Only failed jobs can be retried.");

            job.Status = "Pending";
            job.ErrorMessage = null;
            job.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            await _messageBus.Publish(new TranscodeVideoMessage { JobId = job.Id });
            return NoContent();
        }
    }
}