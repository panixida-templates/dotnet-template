using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api.Middlewares;

public sealed class LoggingMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleWare> _logger;

    public LoggingMiddleWare(RequestDelegate next, ILogger<LoggingMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            string personLogin = context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            string personId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            using (_logger.BeginScope("{personLogin}, {personId}", personLogin, personId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}