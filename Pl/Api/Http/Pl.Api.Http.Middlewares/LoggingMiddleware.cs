using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace Pl.Api.Http.Middlewares;

public sealed class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            string personLogin = context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            string personId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            using (logger.BeginScope("{personLogin}, {personId}", personLogin, personId))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
