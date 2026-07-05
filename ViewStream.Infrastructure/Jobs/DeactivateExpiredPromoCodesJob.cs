using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Daily Hangfire job that deactivates promo codes whose ValidUntil date has passed
    /// or whose usage count has reached MaxUses.
    /// </summary>
    public class DeactivateExpiredPromoCodesJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeactivateExpiredPromoCodesJob> _logger;

        public DeactivateExpiredPromoCodesJob(IServiceScopeFactory scopeFactory, ILogger<DeactivateExpiredPromoCodesJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Deactivate expired promo codes job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Find promo codes that are still "active" but have expired or reached max uses
            var expiredCodes = await unitOfWork.PromoCodes.FindAsync(
                pc => pc.IsActive == true &&
                      ((pc.ValidUntil.HasValue && pc.ValidUntil.Value < today) ||
                       (pc.MaxUses.HasValue && pc.CurrentUses >= pc.MaxUses)));

            foreach (var code in expiredCodes)
            {
                code.IsActive = false;
                unitOfWork.PromoCodes.Update(code);
                _logger.LogInformation("Deactivated promo code '{Code}' (ValidUntil={Until}, Uses={Uses}/{Max})",
                    code.Code, code.ValidUntil, code.CurrentUses, code.MaxUses);
            }

            if (expiredCodes.Any())
                await unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deactivate expired promo codes job completed. {Count} codes deactivated.",
                expiredCodes.Count());
        }
    }
}
