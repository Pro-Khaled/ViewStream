using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Text.Json;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Behaviors;

public class ErrorLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ISystemLogService _systemLog;
    private readonly ILogger<ErrorLogBehavior<TRequest, TResponse>> _logger;

    public ErrorLogBehavior(ISystemLogService systemLog, ILogger<ErrorLogBehavior<TRequest, TResponse>> logger)
    {
        _systemLog = systemLog;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            // Fire-and-forget logging (don't block the exception propagation)
            _ = Task.Run(() => LogErrorAsync(request, ex), CancellationToken.None);

            // Also log to standard ILogger for immediate visibility
            _logger.LogError(ex, "Error handling {RequestType}", typeof(TRequest).Name);

            // Re-throw to let the global exception middleware handle the HTTP response
            throw;
        }
    }

    private async Task LogErrorAsync(TRequest request, Exception ex)
    {
        try
        {
            // Build error message with custom data from ex.Data
            var errorMessage = ex.Message;
            var customData = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in ex.Data)
            {
                if (entry.Key is string key)
                {
                    customData[key] = entry.Value ?? "null";
                }
            }

            if (customData.Any())
            {
                errorMessage += $" | CustomData: {JsonSerializer.Serialize(customData)}";
            }

            // Attempt to extract UserId if the request implements IHasUserId (optional)
            long? userId = null;
            if (request is IHasUserId hasUserId)
            {
                userId = hasUserId.UserId;
            }

            var dto = new CreateErrorLogDto
            {
                UserId = userId,
                ErrorCode = $"{typeof(TRequest).Name}_FAILED",
                ErrorMessage = errorMessage,
                StackTrace = ex.StackTrace,
                Endpoint = typeof(TRequest).Name
            };

            await _systemLog.LogErrorAsync(dto, CancellationToken.None);
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Failed to log error to database for {RequestType}", typeof(TRequest).Name);
        }
    }
}

// Optional interface for requests that carry a UserId
public interface IHasUserId
{
    long? UserId { get; }
}