using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Commands.PersonalizedRow.RegenerateRecommendations;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Hangfire recurring job that regenerates recommendations for all active profiles.
    /// Runs daily at 3 AM.
    /// </summary>
    public class RegenerateRecommendationsJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<RegenerateRecommendationsJob> _logger;

        public RegenerateRecommendationsJob(IUnitOfWork unitOfWork, IMediator mediator, ILogger<RegenerateRecommendationsJob> logger)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Starting RegenerateRecommendationsJob...");
            try
            {
                var profiles = await _unitOfWork.Repository<Profile>().FindAsync(p => p.IsDeleted != true);
                foreach (var profile in profiles)
                {
                    _logger.LogInformation("Triggering recommendations regeneration for ProfileId: {ProfileId}", profile.Id);
                    // ActorUserId = 0 represents the background system
                    await _mediator.Send(new RegenerateRecommendationsCommand(profile.Id, 0));
                }
                _logger.LogInformation("RegenerateRecommendationsJob completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegenerateRecommendationsJob failed.");
                throw;
            }
        }
    }
}
