using System.Security.Claims;
using System.Text.Json;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log to developer console/file
            _logger.LogError(ex, "Unhandled exception at {Endpoint}", context.Request.Path);

            // Fire-and-forget logging to ErrorLog table (via a fresh scope)
            _ = Task.Run(() => LogErrorToDatabaseAsync(context, ex));

            // Return standardized JSON error response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new { message = "An unexpected error occurred. Please try again later." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private async Task LogErrorToDatabaseAsync(HttpContext context, Exception ex)
    {
        try
        {
            // Create a scope to resolve scoped services
            using var scope = _scopeFactory.CreateScope();
            var systemLog = scope.ServiceProvider.GetRequiredService<ISystemLogService>();

            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var dto = new CreateErrorLogDto
            {
                UserId = long.TryParse(userId, out var id) ? id : null,
                ErrorCode = "UNHANDLED_EXCEPTION",
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                Endpoint = $"{context.Request.Method} {context.Request.Path}"
            };

            // Include custom data from ex.Data if present
            if (ex.Data.Count > 0)
            {
                var customData = new Dictionary<string, object>();
                foreach (System.Collections.DictionaryEntry entry in ex.Data)
                {
                    if (entry.Key is string key)
                        customData[key] = entry.Value ?? "null";
                }
                if (customData.Any())
                {
                    dto.ErrorMessage += $" | CustomData: {JsonSerializer.Serialize(customData)}";
                }
            }

            await systemLog.LogErrorAsync(dto, CancellationToken.None);
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Failed to log error to database");
        }
    }
}