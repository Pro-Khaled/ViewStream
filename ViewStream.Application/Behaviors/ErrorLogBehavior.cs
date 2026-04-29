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
            // Build the error DTO (capture custom data and UserId if available)
            var errorMessage = ex.Message;
            var customData = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in ex.Data)
            {
                if (entry.Key is string key)
                    customData[key] = entry.Value ?? "null";
            }
            if (customData.Any())
                errorMessage += $" | CustomData: {JsonSerializer.Serialize(customData)}";

            long? userId = null;
            if (request is IHasUserId hasUserId)
                userId = hasUserId.UserId;

            var dto = new CreateErrorLogDto
            {
                UserId = userId,
                ErrorCode = $"{typeof(TRequest).Name}_FAILED",
                ErrorMessage = errorMessage,
                StackTrace = ex.StackTrace,
                Endpoint = typeof(TRequest).Name
            };

            // Fire‑and‑forget – the logging service opens its own safe scope
            _ = _systemLog.LogErrorAsync(dto, CancellationToken.None);

            _logger.LogError(ex, "Error handling {RequestType}", typeof(TRequest).Name);
            throw; // re‑throw for the global exception middleware
        }
    }
}

// Optional interface – remains the same
public interface IHasUserId
{
    long? UserId { get; }
}