using Hangfire;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Hangfire state election filter that detects consecutive job failures
    /// and sends alert emails to admin after 3 failures.
    /// </summary>
    public class HangfireFailureAlertFilter : Hangfire.States.IElectStateFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireFailureAlertFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnStateElection(Hangfire.States.ElectStateContext context)
        {
            if (context.CandidateState is Hangfire.States.FailedState failedState)
            {
                var retryCount = context.GetJobParameter<int>("RetryCount");
                if (retryCount >= 3)
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var emailService = scope.ServiceProvider.GetService<IEmailService>();
                        var logger = scope.ServiceProvider.GetService<ILogger<HangfireFailureAlertFilter>>();

                        var jobName = context.BackgroundJob.Job?.Type?.Name ?? "Unknown";
                        var errorMessage = failedState.Exception?.Message ?? "No details";

                        logger?.LogError("Hangfire job {JobName} failed {RetryCount} times. Error: {Error}",
                            jobName, retryCount, errorMessage);

                        emailService?.SendNotificationAsync(
                            "admin@viewstream.com",
                            $"[ALERT] Hangfire Job Failure: {jobName}",
                            $"The job '{jobName}' has failed {retryCount} consecutive times.\n\n" +
                            $"Last error: {errorMessage}\n\n" +
                            $"Job ID: {context.BackgroundJob.Id}\n" +
                            $"Time: {DateTime.UtcNow:O}")
                            .GetAwaiter().GetResult();
                    }
                    catch
                    {
                        // Swallow — failure alerting should not break the job pipeline
                    }
                }
            }
        }
    }
}
